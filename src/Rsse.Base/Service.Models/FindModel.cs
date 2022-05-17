using System.Collections.Concurrent;
using RandomSongSearchEngine.Data.Repository.Contracts;
using RandomSongSearchEngine.Infrastructure.Cache.Contracts;
using RandomSongSearchEngine.Infrastructure.Engine;
using RandomSongSearchEngine.Infrastructure.Engine.Contracts;

namespace RandomSongSearchEngine.Service.Models;

public class FindModel
{
    private readonly IServiceScope _scope;
    private readonly ILogger<FindModel> _logger;
    private readonly ConcurrentDictionary<int, List<int>> _undefinedCache;
    private readonly ConcurrentDictionary<int, List<int>> _definedCache;
    private readonly object _obj = new();

    public FindModel(IServiceScope scope)
    {
        _scope = scope;
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<FindModel>>();
        _undefinedCache = scope.ServiceProvider.GetRequiredService<ICacheRepository>().GetUndefinedCache();
        _definedCache = scope.ServiceProvider.GetRequiredService<ICacheRepository>().GetDefinedCache();
    }

    // TODO: сделать индекс, зависящий от количества слов в источнике и сортировку ответов по индексу
    public Dictionary<int, double> Find(string text)
    {
        /*if (_undefinedCache.IsEmpty)
        {
            InitializeCaches();
        }
        */

        var result = new Dictionary<int, double>();

        // I. defined поиск: 0.8D
        const double defined = 0.8D;
        // II. undefined поиск: 0.4D
        const double undefined = 0.6D; // 0.6 .. 0.75

        var undefinedSearch = true;

        var processor = _scope.ServiceProvider.GetRequiredService<ITextProcessor>();

        processor.Setup(ConsonantChain.Defined);

        var hash = processor.CleanUpString(text);

        if (hash.Count == 0)
        {
            // песни вида "123 456" не ищем, так как "найдёт" весь каталог
            return result;
        }

        var item = processor.GetHashSetFromStrings(hash);

        foreach (var (key, value) in _definedCache)
        {
            var metric = processor.GetComparisionMetric(value, item);

            // I. 100% совпадение defined, undefined можно не искать
            if (metric == item.Count)
            {
                undefinedSearch = false;
                result.Add(key, metric * (1000 / value.Count));
                continue;
            }

            // II. defined% совпадение
            if (metric >= item.Count * defined)
            {
                result.Add(key, metric * (100 / value.Count));
            }
        }

        if (!undefinedSearch)
        {
            return result;
        }

        processor.Setup(ConsonantChain.Undefined);

        hash = processor.CleanUpString(text);

        item = processor.GetHashSetFromStrings(hash);
        // убираем дубликаты слов для intersect - это меняет результаты поиска
        item = item.ToHashSet().ToList();

        foreach (var (key, value) in _undefinedCache)
        {
            var metric = processor.GetComparisionMetric(value, item);

            // III. 100% совпадение undefined
            if (metric == item.Count)
            {
                result.TryAdd(key, metric * (10 / value.Count));
                continue;
            }

            // IV. undefined% совпадение
            if (metric >= item.Count * undefined)
            {
                result.TryAdd(key, metric * (1 / value.Count));
            }
        }

        return result;
    }

    /*private void InitializeCaches()
    {
        using var repo = _scope.ServiceProvider.GetRequiredService<IDataRepository>();
        var processor = _scope.ServiceProvider.GetRequiredService<ITextProcessor>();

        // TODO: стоит ли создавать индекс под блокировкой?
        lock (_obj)
        {
            try
            {
                var texts = repo.ReadAllSongs();

                foreach (var text in texts)
                {
                    // undefined hash
                    processor.Setup(ConsonantChain.Undefined);

                    var song = processor.ConvertStringToText(text);

                    song.Title.ForEach(t => song.Words.Add(t));

                    var undefinedHash = processor.GetHashSetFromStrings(song.Words); // название - в конце списка

                    if (!_undefinedCache.TryAdd(song.Number, undefinedHash))
                    {
                        throw new MethodAccessException("[FindModel Create Caches: undefined failed]");
                    }

                    // defined hash
                    processor.Setup(ConsonantChain.Defined);

                    song = processor.ConvertStringToText(text);

                    song.Title.ForEach(t => song.Words.Add(t));

                    var definedHash = processor.GetHashSetFromStrings(song.Words);

                    if (!_definedCache.TryAdd(song.Number, definedHash))
                    {
                        throw new MethodAccessException("[FindModel Create Caches: defined failed]");
                    }
                }

                GC.Collect();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FindModel: OnGet Error]");
            }
        }*/
}