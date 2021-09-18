using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Dto;
using RandomSongSearchEngine.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace RandomSongSearchEngine.Extensions
{
    public class DatabaseAccess : IDatabaseAccess, IAsyncDisposable, IDisposable
    {
        private RsseContext _context;

        public DatabaseAccess(IServiceProvider serviceProvider)
        {
            _context = serviceProvider.GetRequiredService<RsseContext>();
        }

        public async Task НуЕгоНаХуй()
        {
            await DisposeAsync();
        }

        public async Task<UserEntity> GetUser(LoginDto login)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == login.Password);
        }

        public async Task<int> ReadTextsCountAsync()
        {
            return await _context.Text.CountAsync();
        }

        public IQueryable<int> SelectSongIds(int[] checkedGenres)
        {
            //TODO определить какой лучше:
            //IQueryable<int> songsCollection = database.GenreText//
            //    .Where(s => chosenOnes.Contains(s.GenreInGenreText.GenreID))
            //    .Select(s => s.TextInGenreText.TextID);

            IQueryable<int> songsCollection = from a in _context.Text
                                              where a.GenreTextInText.Any(c => checkedGenres.Contains(c.GenreId))
                                              select a.TextId;
            return songsCollection;
        }

        public async Task<List<string>> ReadGenreListAsync()
        {
            List<string> genreListResponse = new List<string>();
            List<Tuple<string, int>> genreList = await ReadGenreList().ToListAsync();
            foreach (var genreAndAmount in genreList)
            {
                genreListResponse.Add(genreAndAmount.Item2 > 0 ? genreAndAmount.Item1 + ": " + genreAndAmount.Item2 : genreAndAmount.Item1);
            }
            return genreListResponse;
        }

        public IQueryable<Tuple<string, int>> ReadCatalogPage(int savedLastViewedPage, int pageSize)
        {
            IQueryable<Tuple<string, int>> titleAndTextId = _context.Text
                .OrderBy(s => s.Title)
                .Skip((savedLastViewedPage - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new Tuple<string, int>(s.Title, s.TextId))
                .AsNoTracking();

            return titleAndTextId;
        }

        public IQueryable<Tuple<string, string>> ReadSong(int textId)
        {
            IQueryable<Tuple<string, string>> titleAndText = _context.Text
                .Where(p => p.TextId == textId)
                .Select(s => new Tuple<string, string>(s.Song, s.Title))
                .AsNoTracking();
            return titleAndText;
        }

        public IQueryable<int> ReadSongGenres(int savedTextId)
        {
            IQueryable<int> checkedList = _context.GenreText
                .Where(p => p.TextInGenreText.TextId == savedTextId)
                .Select(s => s.GenreId);
            return checkedList;
        }

        public async Task UpdateSongAsync(List<int> originalCheckboxes, SongDto song)
        {
            // если имя заменено на существующее, то залоггируется исключение.
            // быстрой проверки нет - ресурсоёмко и ошибка редкая.
            // Id жанров из бд и номера кнопок с фронта совпадают
            await using IDbContextTransaction t = await _context.Database.BeginTransactionAsync();
            try
            {
                TextEntity text = await _context.Text.FindAsync(song.Id);
                if (text == null)
                {
                    throw new Exception("[NULL in Text]");
                }
                HashSet<int> forAddition = song.SongGenres.ToHashSet();
                HashSet<int> forDelete = originalCheckboxes.ToHashSet();
                List<int> except = forAddition.Intersect(forDelete).ToList();
                forAddition.ExceptWith(except);
                forDelete.ExceptWith(except);
                // дешевле просто откатить транзакцию без механизма исключений
                CheckGenresExistsError(song.Id, forAddition);
                text.Title = song.Title;
                text.Song = song.Text;
                _context.Text.Update(text);
                _context.GenreText.RemoveRange(_context.GenreText.Where(f => f.TextId == song.Id && forDelete.Contains(f.GenreId)));
                await _context.GenreText.AddRangeAsync(forAddition.Select(genre => new GenreTextEntity { TextId = song.Id, GenreId = genre }));
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
                throw new Exception("[UpdateSongSqlAsync Method]", ex);
            }
        }

        public async Task<int> CreateSongAsync(SongDto song)
        {
            await using IDbContextTransaction t = await _context.Database.BeginTransactionAsync();
            try
            {
                // дешевле просто откатить транзакцию без механизма исключений
                CheckNameExistsError(song.Title);
                TextEntity addition = new TextEntity { Title = song.Title, Song = song.Text };
                await _context.Text.AddAsync(addition);
                await _context.SaveChangesAsync();
                await _context.GenreText.AddRangeAsync(song.SongGenres.Select(genre => new GenreTextEntity { TextId = addition.TextId, GenreId = genre }));
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
                throw new Exception("[CreateSongSqlAsync Method]", ex);
            }

            return song.Id;
        }

        public async Task<int> DeleteSongAsync(int songId)
        {
            await using IDbContextTransaction t = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = 0;
                var song = await _context.Text.FindAsync(songId);
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
                throw new Exception("[OnDeleteSql Method]", ex);
            }
        }

        // Проверка консистентости данных по названию песни
        private void CheckNameExistsError(string title)
        {
            int r = _context.Text
                .Where(p => p.Title == title)
                .AsNoTracking()
                .Count();
            if (r > 0)
            {
                throw new DataExistsException("[Browser Refresh or Name Exists Error]");
            }
        }

        // Проверка консистнентности данных по Id песни и категориям для добавления
        private void CheckGenresExistsError(int textId, HashSet<int> forAddition)
        {
            if (forAddition.Count > 0)
            {
                int r = _context.GenreText
                    .Where(p => p.TextId == textId && p.GenreId == forAddition.First())
                    .AsNoTracking()
                    .Count();
                if (r > 0)
                {
                    throw new DataExistsException("[Browser Refresh or Genre Exists Error]");
                }
            }
        }

        // Выборка названий жанров и количества песен в каждом из них
        private IQueryable<Tuple<string, int>> ReadGenreList()
        {
            IQueryable<Tuple<string, int>> res = _context.Genre
                .Select(g => new Tuple<string, int>(g.Genre, g.GenreTextInGenre.Count))
                .AsNoTracking();
            return res;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual async Task DisposeAsyncCore()
        {
            if (_context != null)
            {
                await _context.DisposeAsync().ConfigureAwait(false);
            }

            _context = null;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }

            _context = null;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}