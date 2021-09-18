using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Dto;
using RandomSongSearchEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    public class UpdateModel
    {
        private IServiceScope _scope { get; }
        private ILogger<UpdateModel> _logger { get; }
        public UpdateModel(IServiceScope scope)
        {
            _scope = scope;
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<UpdateModel>>();
        }

        public async Task<SongDto> ReadOriginalSongAsync(int originalSongId)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<IDatabaseAccess>();
            try
            {
                //if (SavedTextId == 0) throw new NotImplementedException();
                string textResponse = "";
                string titleResponse = "";
                List<Tuple<string, string>> song = await database.ReadSong(originalSongId).ToListAsync();
                if (song.Count > 0)
                {
                    textResponse = song[0].Item1;
                    titleResponse = song[0].Item2;
                }

                List<string> genreListResponse = await database.ReadGenreListAsync();
                List<int> songGenres = await database.ReadSongGenres(originalSongId).ToListAsync();
                List<string> songGenresResponse = new List<string>();
                for (int i = 0; i < genreListResponse.Count; i++)
                {
                    songGenresResponse.Add("unchecked");
                }
                foreach (int i in songGenres)
                {
                    songGenresResponse[i - 1] = "checked";
                }
                return new SongDto(genreListResponse, originalSongId, textResponse, titleResponse, songGenresResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChangeTextModel: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[ChangeTextModel: OnGet Error]" };
            }
        }

        public async Task<SongDto> UpdateSongAsync(SongDto updatedSong)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<IDatabaseAccess>();
            try
            {
                if (updatedSong.SongGenres == null || updatedSong.Text == null || updatedSong.Title == null
                    || updatedSong.SongGenres.Count == 0 || updatedSong.Text == "" || updatedSong.Title == "")
                {
                    return await ReadOriginalSongAsync(updatedSong.Id);
                }
                List<int> originalGenres = await database.ReadSongGenres(updatedSong.Id).ToListAsync();
                await database.UpdateSongAsync(originalGenres, updatedSong);
                return await ReadOriginalSongAsync(updatedSong.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChangeTextModel: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[ChangeTextModel: OnPost Error]" };
            }
        }
    }
}
