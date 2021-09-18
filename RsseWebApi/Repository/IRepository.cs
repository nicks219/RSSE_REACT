using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Repository
{
    public interface IRepository : IAsyncDisposable, IDisposable
    {
        Task<int> CreateSongAsync(SongDto dt);
        IQueryable<Tuple<string, string>> ReadSong(int textId);
        Task UpdateSongAsync(List<int> originalCheckboxes, SongDto dt);
        Task<int> DeleteSongAsync(int songId);
        Task<UserEntity> GetUser(LoginDto dt);
        IQueryable<Tuple<string, int>> ReadCatalogPage(int lastPage, int pageSize);
        Task<List<string>> ReadGenreListAsync();
        Task<int> ReadTextsCountAsync();
        IQueryable<int> ReadSongGenres(int textId);
        IQueryable<int> SelectAllSongsInGenres(int[] checkedGenres);
    }
}