using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Dto;
using RandomSongSearchEngine.Repository;
using RandomSongSearchEngine.Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    public class ReadModel
    {
        private IServiceScope _scope { get; }
        private ILogger<ReadModel> _logger { get; }

        public ReadModel(IServiceScope serviceScope)
        {
            _scope = serviceScope;
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<ReadModel>>();
        }

        public async Task<SongDto> ReadGenreListAsync()
        {
            await using var repo = _scope.ServiceProvider.GetRequiredService<IRepository>();
            try
            {
                List<string> genreListResponse = await repo.ReadGenreListAsync();
                return new SongDto(genreListResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[IndexModel: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[IndexModel: OnGet Error]" };
            }
        }

        public async Task<SongDto> ReadRandomSongAsync(SongDto request)
        {
            string textResponse = "";
            string titleResponse = "";
            int songId = 0;
            await using var repo = _scope.ServiceProvider.GetRequiredService<IRepository>();
            try
            {
                if (request.SongGenres != null && request.SongGenres.Count != 0)
                {
                    songId = await repo.ReadRandomIdAsync(request.SongGenres);
                    if (songId != 0)
                    {
                        var song = await repo.ReadSong(songId).ToListAsync();
                        if (song.Count > 0)
                        {
                            textResponse = song[0].Item1;
                            titleResponse = song[0].Item2;
                        }
                    }
                }

                List<string> genreListResponse = await repo.ReadGenreListAsync();
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