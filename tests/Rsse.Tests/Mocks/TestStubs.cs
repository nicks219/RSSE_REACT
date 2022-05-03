namespace RandomSongSearchEngine.Tests.Mocks;
/*
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
        var res = _dictionary.Remove(songId);
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
        var user = new UserEntity();
        return new Task<UserEntity>(() => user);
    }

    public IQueryable<Tuple<string, int>> ReadCatalogPage(int lastPage, int pageSize)
    {
        var q = new List<Tuple<string, int>>
        {
            new(_dictionary[1].Item1, 1)
        };
        return q.AsQueryable();
    }

    public Task<List<string>> ReadGenreListAsync()
    {
        return new Task<List<string>>(() => new List<string>() {"Rock", "Pop", "Jazz"});
    }

    public IQueryable<Tuple<string, string>> ReadSong(int textId)
    {
        var q = new List<Tuple<string, string>>
        {
            _dictionary[textId]
        };
        return q.AsQueryable();
    }

    public IQueryable<int> ReadSongGenres(int textId)
    {
        var l = new List<int>();
        var r = _dictionary[textId];
        if (r == null)
        {
            l.Add(1);
            l.Add(2);
        }

        return l.AsQueryable();
    }

    public Task<int> ReadTextsCountAsync()
    {
        return new Task<int>(() => _dictionary.Count);
    }

    public IQueryable<int> SelectAllSongsInGenres(int[] checkedGenres)
    {
        var l = new List<int> {1, 2, 3};
        return l.AsQueryable();
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
*/
