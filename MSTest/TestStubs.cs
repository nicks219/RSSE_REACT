using Microsoft.Extensions.DependencyInjection;
using RandomSongSearchEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Data.Repository.Contracts;

namespace MSTest;

// что это за класс?
public class StubRepository : IRepository
{
    private int _id;
    private Dictionary<int, Tuple<string, string>> _dictionary;

    public Task<int> CreateSongAsync(SongDto dt)
    {
        _dictionary.Add(_id, new Tuple<string, string>(dt.Title, dt.Text));
        _id++;
        return new Task<int>(() => _id - 1);
    }

    public Task<int> DeleteSongAsync(int songId)
    {
        bool res = _dictionary.Remove(songId);
        return new Task<int>(() => Convert.ToInt32(res));
    }

    public void Dispose()
    {
        _dictionary = null;
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }

    //Login Ok
    public Task<UserEntity> GetUser(LoginDto dt)
    {
        UserEntity user = new UserEntity();
        return new Task<UserEntity>(() => user);
    }

    public IQueryable<Tuple<string, int>> ReadCatalogPage(int lastPage, int pageSize)
    {
        List<Tuple<string, int>> q = new List<Tuple<string, int>>()
        {
            new Tuple<string, int>(_dictionary[1].Item1, 1)
        };
        return (IQueryable<Tuple<string, int>>) q;
    }

    public Task<List<string>> ReadGenreListAsync()
    {
        return new Task<List<string>>(() => new List<string>() {"Rock", "Pop", "Jazz"});
    }

    public IQueryable<Tuple<string, string>> ReadSong(int textId)
    {
        List<Tuple<string, string>> q = new List<Tuple<string, string>>
        {
            _dictionary[textId]
        };
        return (IQueryable<Tuple<string, string>>) q;
    }

    public IQueryable<int> ReadSongGenres(int textId)
    {
        List<int> l = new List<int>();
        var r = _dictionary[textId];
        if (r == null)
        {
            l.Add(1);
            l.Add(2);
        }

        return (IQueryable<int>) l;
    }

    public Task<int> ReadTextsCountAsync()
    {
        return new Task<int>(() => _dictionary.Count);
    }

    public IQueryable<int> SelectAllSongsInGenres(int[] checkedGenres)
    {
        List<int> l = new List<int>() {1, 2, 3};
        return (IQueryable<int>) l;
    }

    public Task UpdateSongAsync(List<int> originalCheckboxes, SongDto dt)
    {
        _dictionary[dt.Id] = new Tuple<string, string>(dt.Title, dt.Text);
        return new Task(() => Console.Write(""));
    }
}

public class FakeServiceScopeFactory : IServiceScopeFactory
{
    private readonly IServiceScope _serviceScope;

    public IServiceScope CreateScope()
    {
        return _serviceScope;
    }

    public FakeServiceScopeFactory(IServiceScope serviceScope)
    {
        _serviceScope = serviceScope;
    }
}
