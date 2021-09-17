using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Dto;
using RandomSongSearchEngine.Extensions;
using RandomSongSearchEngine.Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    public class ReadModel : BaseModel
    {
        private IServiceScope _scope { get; }
        private ILogger<ReadModel> _logger { get; }
        public ReadModel(IServiceScope serviceScope)
        {
            _scope = serviceScope;
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<ReadModel>>();
        }

        public async Task<SongDto> OnGetAsync()
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                List<string> genreListResponse = await GetGenreListAsync(database: database);
                return new SongDto(genreListResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IndexModel: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[IndexModel: OnGet Error]" };
            }
        }

        public async Task<SongDto> OnPostAsync(SongDto dto)
        {
            string textResponse = "";
            string titleResponse = "";
            int songId = 0;
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                if (dto.SongGenresRequest != null && dto.SongGenresRequest.Count != 0)
                {
                    songId = await database.GetRandomIdAsync(dto.SongGenresRequest);
                    if (songId != 0)
                    {
                        var song = await database.ReadSongSql(songId).ToListAsync();
                        if (song.Count > 0)
                        {
                            textResponse = song[0].Item1;
                            titleResponse = song[0].Item2;
                        }
                    }
                }

                List<string> genreListResponse = await GetGenreListAsync(database: database);
                return new SongDto(genreListResponse, songId, textResponse, titleResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IndexModel: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[IndexModel: OnPost Error]" };
            }
        }
    }
}