using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    public class UpdateModel : BaseModel
    {
        private IServiceScope _scope { get; }
        private ILogger<UpdateModel> _logger { get; }
        public UpdateModel(IServiceScope scope)
        {
            _scope = scope;
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<UpdateModel>>();
        }

        public async Task<SongDto> OnGetAsync(int songId)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                //if (SavedTextId == 0) throw new NotImplementedException();
                string textResponse = "";
                string titleResponse = "";
                List<Tuple<string, string>> song = await database.ReadSongSql(songId).ToListAsync();
                if (song.Count > 0)
                {
                    textResponse = song[0].Item1;
                    titleResponse = song[0].Item2;
                }

                List<string> genreListResponse = await GetGenreListAsync(database: database);
                List<int> songGenres = await database.ReadSongGenresSql(songId).ToListAsync();
                List<string> songGenresResponse = new List<string>();
                for (int i = 0; i < genreListResponse.Count; i++)
                {
                    songGenresResponse.Add("unchecked");
                }
                foreach (int i in songGenres)
                {
                    songGenresResponse[i - 1] = "checked";
                }
                return new SongDto(genreListResponse, songId, textResponse, titleResponse, songGenresResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChangeTextModel: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[ChangeTextModel: OnGet Error]" };
            }
        }

        public async Task<SongDto> OnPostAsync(SongDto dto)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                if (dto.SongGenresRequest == null || dto.TextRequest == null || dto.TitleRequest == null
                    || dto.SongGenresRequest.Count == 0 || dto.TextRequest == "" || dto.TitleRequest == "")
                {
                    return await OnGetAsync(dto.SongId);
                }
                List<int> originalGenres = await database.ReadSongGenresSql(dto.SongId).ToListAsync();
                await database.UpdateSongSqlAsync(originalGenres, dto);
                return await OnGetAsync(dto.SongId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChangeTextModel: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[ChangeTextModel: OnPost Error]" };
            }
        }
    }
}
