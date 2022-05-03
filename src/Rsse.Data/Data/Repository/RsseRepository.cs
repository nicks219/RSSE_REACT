using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Data.Repository.Contracts;
using RandomSongSearchEngine.Data.Repository.Exceptions;

namespace RandomSongSearchEngine.Data.Repository;

public class RsseRepository : IRepository
{
    private readonly RsseContext _context;

    public RsseRepository(IServiceProvider serviceProvider)
    {
        _context = serviceProvider.GetRequiredService<RsseContext>();
    }

    public IQueryable<int> SelectAllSongsInGenres(int[] checkedGenres)
    {
        // TODO: определить какой метод лучше
        // IQueryable<int> songsCollection = database.GenreText//
        //    .Where(s => chosenOnes.Contains(s.GenreInGenreText.GenreID))
        //    .Select(s => s.TextInGenreText.TextID);

        IQueryable<int> songsForRandomizer = _context.Text!
            .Where(a => a.GenreTextInText!.Any(c => checkedGenres.Contains(c.GenreId)))
            .Select(a => a.TextId);
        return songsForRandomizer;
    }

    public IQueryable<Tuple<string, int>> ReadCatalogPage(int lastPage, int pageSize)
    {
        IQueryable<Tuple<string, int>> titlesAndIdsList = _context.Text!
            .OrderBy(s => s.Title)
            .Skip((lastPage - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new Tuple<string, int>(s.Title!, s.TextId))
            .AsNoTracking();

        return titlesAndIdsList;
    }

    public IQueryable<Tuple<string, string>> ReadSong(int textId)
    {
        IQueryable<Tuple<string, string>> titleAndText = _context.Text!
            .Where(p => p.TextId == textId)
            .Select(s => new Tuple<string, string>(s.Song!, s.Title!))
            .AsNoTracking();
        return titleAndText;
    }

    public IQueryable<int> ReadSongGenres(int textId)
    {
        IQueryable<int> songGenres = _context.GenreText!
            .Where(p => p.TextInGenreText!.TextId == textId)
            .Select(s => s.GenreId);
        return songGenres;
    }

    public async Task<UserEntity> GetUser(LoginDto login)
    {
        return await _context.Users!.FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == login.Password);
    }

    public async Task<int> ReadTextsCountAsync()
    {
        return await _context.Text!.CountAsync();
    }

    public async Task<List<string>> ReadGenreListAsync()
    {
        List<string> genreListResponse = new List<string>();
        List<Tuple<string, int>> genreList = await ReadGenreList().ToListAsync();
        foreach (var genreAndAmount in genreList)
        {
            genreListResponse.Add(genreAndAmount.Item2 > 0
                ? genreAndAmount.Item1 + ": " + genreAndAmount.Item2
                : genreAndAmount.Item1);
        }

        return genreListResponse;
    }

    public async Task UpdateSongAsync(List<int> originalCheckboxes, SongDto song)
    {
        HashSet<int> forAddition = song.SongGenres!.ToHashSet();
        HashSet<int> forDelete = originalCheckboxes.ToHashSet();
        List<int> except = forAddition.Intersect(forDelete).ToList();
        forAddition.ExceptWith(except);
        forDelete.ExceptWith(except);

        if (await CheckGenresExistsErrorAsync(song.Id, forAddition))
        {
            // название песни остаётся неизменным (constraint)
            // Id жанров и номера кнопок с фронта совпадают
            await using IDbContextTransaction t = await _context.Database.BeginTransactionAsync();
            try
            {
                // дешевле просто откатить или не начинать транзакцию, без механизма исключений
                TextEntity text = await _context.Text!.FindAsync(song.Id);
                if (text == null)
                {
                    throw new Exception("[UpdateSongAsync: Null in Text]");
                }

                text.Title = song.Title;
                text.Song = song.Text;
                _context.Text.Update(text);
                _context.GenreText!.RemoveRange(_context.GenreText.Where(f =>
                    f.TextId == song.Id && forDelete.Contains(f.GenreId)));
                await _context.GenreText.AddRangeAsync(forAddition.Select(genre => new GenreTextEntity
                    {TextId = song.Id, GenreId = genre}));
                await _context.SaveChangesAsync();
                await t.CommitAsync();
            }
            catch (DataExistsException)
            {
                await t.RollbackAsync();
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();
                throw new Exception("[UpdateSongAsync: Repo]", ex);
            }
        }
    }

    public async Task<int> CreateSongAsync(SongDto song)
    {
        if (await CheckNameExistsErrorAsync(song.Title!))
        {
            await using IDbContextTransaction t = await _context.Database.BeginTransactionAsync();
            try
            {
                TextEntity addition = new TextEntity {Title = song.Title, Song = song.Text};
                await _context.Text!.AddAsync(addition);
                await _context.SaveChangesAsync();
                await _context.GenreText!.AddRangeAsync(song.SongGenres!.Select(genre => new GenreTextEntity
                    {TextId = addition.TextId, GenreId = genre}));
                await _context.SaveChangesAsync();
                await t.CommitAsync();
                song.Id = addition.TextId;
            }
            catch (DataExistsException)
            {
                await t.RollbackAsync();
                song.Id = 0;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();
                song.Id = 0;
                throw new Exception("[CreateSongAsync: Repo]", ex);
            }
        }

        return song.Id;
    }

    public async Task<int> DeleteSongAsync(int songId)
    {
        await using IDbContextTransaction t = await _context.Database.BeginTransactionAsync();
        try
        {
            var result = 0;
            var song = await _context.Text!.FindAsync(songId);
            if (song != null)
            {
                _context.Text.Remove(song);
                result = await _context.SaveChangesAsync();
                await t.CommitAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            await t.RollbackAsync();
            throw new Exception("[DeleteSongAsync: Repo]", ex);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    private async Task<bool> CheckNameExistsErrorAsync(string title)
    {
        return !await _context.Text!.AnyAsync(p => p.Title == title);
    }

    private async Task<bool> CheckGenresExistsErrorAsync(int textId, HashSet<int> forAddition)
    {
        if (forAddition.Count > 0)
        {
            if (await _context.GenreText!.AnyAsync(p => p.TextId == textId && p.GenreId == forAddition.First()))
            {
                throw new DataExistsException("[Global Error: Genre Exists Error]");
            }
        }

        return true;
    }

    // Выборка названий жанров и количества песен в каждом из них
    private IQueryable<Tuple<string, int>> ReadGenreList()
    {
        IQueryable<Tuple<string, int>> genreList = _context.Genre!
            .Select(g => new Tuple<string, int>(g.Genre!, g.GenreTextInGenre!.Count))
            .AsNoTracking();
        return genreList;
    }
}