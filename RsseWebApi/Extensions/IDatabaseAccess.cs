using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Extensions
{
    public interface IDatabaseAccess : IAsyncDisposable, IDisposable
    {
        Task НуЕгоНаХуй();
        Task<UserEntity> GetUser(LoginDto dt);
        Task<int> ReadTextsCountAsync();
        Task<int> CreateSongAsync(SongDto dt);
        Task<int> DeleteSongAsync(int songId);
        IQueryable<Tuple<string, int>> ReadCatalogPage(int savedLastViewedPage, int pageSize);
        Task<List<string>> ReadGenreListAsync();
        IQueryable<Tuple<string, string>> ReadSong(int textId);
        IQueryable<int> ReadSongGenres(int savedTextId);
        IQueryable<int> SelectSongIds(int[] checkedGenres);
        Task UpdateSongAsync(List<int> initialCheckboxes, SongDto dt);
    }
}