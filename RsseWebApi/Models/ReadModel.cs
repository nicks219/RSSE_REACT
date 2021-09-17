using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Extensions;
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

        public async Task<SongDto> OnGetReadAsync()
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                List<string> genreListCs = await InitializeGenreListAsync(database: database);
                return new SongDto(genreListCs);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[IndexModel: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[IndexModel: OnGet Error]" };
            }
        }

        public async Task<SongDto> OnPostReadAsync(SongDto dto)
        {
            string textCs = "";
            string titleCs = "";
            int songId = 0;

            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                if (dto.CheckedCheckboxesRequest != null && dto.CheckedCheckboxesRequest.Count != 0)
                {
                    songId = await database.GetRandomIdAsync(dto.CheckedCheckboxesRequest);
                    if (songId != 0)
                    {
                        var song = await database.ReadSongSql(songId).ToListAsync();
                        if (song.Count > 0)
                        {
                            textCs = song[0].Item1;
                            titleCs = song[0].Item2;
                        }
                    }
                }

                List<string> genreListCs = await InitializeGenreListAsync(database: database);
                return new SongDto(genreListCs, songId, textCs, titleCs);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[IndexModel: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[IndexModel: OnPost Error]" };
            }
        }

        private async Task<List<string>> InitializeGenreListAsync(RsseContext database)
        {
            List<string> genreListCs = new List<string>();
            List<Tuple<string, int>> genreList = await database.ReadGenreListSql().ToListAsync();
            foreach (var genreAndAmount in genreList)
            {
                genreListCs.Add(genreAndAmount.Item2 > 0 ? genreAndAmount.Item1 + ": " + genreAndAmount.Item2 : genreAndAmount.Item1);
            }
            return genreListCs;
        }
    }
}