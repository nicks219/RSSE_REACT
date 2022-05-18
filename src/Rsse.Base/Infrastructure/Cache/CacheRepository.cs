using System.Collections.Concurrent;
using RandomSongSearchEngine.Data.Repository.Contracts;
using RandomSongSearchEngine.Infrastructure.Cache.Contracts;
using RandomSongSearchEngine.Infrastructure.Engine;
using RandomSongSearchEngine.Infrastructure.Engine.Contracts;

namespace RandomSongSearchEngine.Infrastructure.Cache;

public class CacheRepository : ICacheRepository
{
    // TODO: в случае ошибок в этом репозитории надо пересоздавать кэш под блокировкой, а не кидать эксепшны
    // TODO: docker restart: always
    private readonly IServiceScopeFactory _factory;
    private readonly  ConcurrentDictionary<int, List<int>> _undefinedCache;
    private readonly  ConcurrentDictionary<int, List<int>> _definedCache;
    private readonly object _obj = new();

    public CacheRepository(IServiceScopeFactory factory)
    {
        _factory = factory;
        _undefinedCache = new ConcurrentDictionary<int, List<int>>();
        _definedCache = new ConcurrentDictionary<int, List<int>>();
        
        Initialize();
    }
    
    public ConcurrentDictionary<int, List<int>> GetUndefinedCache()
    {
        return _undefinedCache;
    }
    
    public ConcurrentDictionary<int, List<int>> GetDefinedCache()
    {
        return _definedCache;
    }

    public void Delete(int id)
    {
        if (_undefinedCache.TryRemove(id, out _) && _definedCache.TryRemove(id, out _))
        {
            return;
        }
        
        throw new MethodAccessException("[ConcurrentCacheDelete: failed]");
    }

    public void Create(int id, string text)
    {
        using var scope = _factory.CreateScope();

        var processor = scope.ServiceProvider.GetRequiredService<ITextProcessor>();
        
        // undefined hash
        processor.Setup(ConsonantChain.Undefined);
                
        var song = processor.ConvertStringToText(text);
                
        song.Title.ForEach(t => song.Words.Add(t));
                
        var undefinedHash = processor.GetHashSetFromStrings(song.Words); // название - в конце списка

        if (!_undefinedCache.TryAdd(song.Number, undefinedHash))
        {
            throw new MethodAccessException("[ConcurrentCacheCreate: undefined failed]");
        }
                
        // defined hash
        processor.Setup(ConsonantChain.Defined);
                
        song = processor.ConvertStringToText(text);
                
        song.Title.ForEach(t => song.Words.Add(t));

        var definedHash = processor.GetHashSetFromStrings(song.Words);

        if (!_definedCache.TryAdd(song.Number, definedHash))
        {
            throw new MethodAccessException("[ConcurrentCacheCreate: defined failed]");
        }
    }

    public void Update(int id, string text)
    {
        using var scope = _factory.CreateScope();

        var processor = scope.ServiceProvider.GetRequiredService<ITextProcessor>();
        
        // undefined hash
        processor.Setup(ConsonantChain.Undefined);
                
        var song = processor.ConvertStringToText(text);
                
        song.Title.ForEach(t => song.Words.Add(t));
                
        var undefinedHash = processor.GetHashSetFromStrings(song.Words); // название - в конце списка

        if (_undefinedCache.TryGetValue(song.Number, out var oldHash))
        {
            if (!_undefinedCache.TryUpdate(song.Number, undefinedHash, oldHash))
            {
                throw new MethodAccessException("[ConcurrentCacheUpdate: undefined failed on stage 2]");
            }
        }
        else
        {
            throw new MethodAccessException("[ConcurrentCacheUpdate: undefined failed on stage 1]");
        }
                
        // defined hash
        processor.Setup(ConsonantChain.Defined);
                
        song = processor.ConvertStringToText(text);
                
        song.Title.ForEach(t => song.Words.Add(t));

        var definedHash = processor.GetHashSetFromStrings(song.Words);

        if (_definedCache.TryGetValue(song.Number, out oldHash))
        {
            if (!_definedCache.TryUpdate(song.Number, definedHash, oldHash))
            {
                throw new MethodAccessException("[ConcurrentCacheUpdate: defined failed on stage 2]");
            }
        }
        else
        {
            throw new MethodAccessException("[ConcurrentCacheUpdate: defined failed on stage 1]");
        }
    }
    
    private void Initialize()
    {
        using var scope = _factory.CreateScope();
        
        using var repo = scope.ServiceProvider.GetRequiredService<IDataRepository>();
        
        var processor = scope.ServiceProvider.GetRequiredService<ITextProcessor>();
        
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CacheRepository>>();

        // TODO: блокировка?
        lock (_obj)
        {
            if (!_undefinedCache.IsEmpty)
            {
                throw new Exception("[Cache Repository: serial initialization]");
            }
            
            try
            {
                var texts = repo.ReadAllSongs();

                foreach (var text in texts)
                {
                    // undefined hash line
                    processor.Setup(ConsonantChain.Undefined);

                    var song = processor.ConvertStringToText(text);

                    song.Title.ForEach(t => song.Words.Add(t));

                    var undefinedHash = processor.GetHashSetFromStrings(song.Words); // название - в конце списка

                    if (!_undefinedCache.TryAdd(song.Number, undefinedHash))
                    {
                        throw new MethodAccessException("[Cache Repository Init: undefined failed]");
                    }

                    // defined hash line
                    processor.Setup(ConsonantChain.Defined);

                    song = processor.ConvertStringToText(text);

                    song.Title.ForEach(t => song.Words.Add(t));

                    var definedHash = processor.GetHashSetFromStrings(song.Words);

                    if (!_definedCache.TryAdd(song.Number, definedHash))
                    {
                        throw new MethodAccessException("[Cache Repository Init: defined failed]");
                    }
                }

                GC.Collect();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Cache Repository Init: OnGet Error]");
            }
        }
    }
}