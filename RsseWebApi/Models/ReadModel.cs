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
        private ILogger<SongModel> _logger { get; }
        public ReadModel(IServiceScope serviceScope)
        {
            _scope = serviceScope;
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<SongModel>>();
        }

        public async Task<SongDto> OnGetReadAsync()
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                List<string> genreListCs = await InitializeCheckboxesAsync(database: database);
                return new SongDto(genreListCs);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[IndexModel: OnGet Error]");
                return new SongDto() { ErrorMessageCs = "[IndexModel: OnGet Error]" };
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
                if (dto.CheckedCheckboxesJs != null && dto.CheckedCheckboxesJs.Count != 0)
                {
                    songId = await database.GetRandomIdAsync(dto.CheckedCheckboxesJs);
                    if (songId != 0)
                    {
                        var r = await database.ReadSongSql(songId).ToListAsync();
                        if (r.Count > 0)
                        {
                            textCs = r[0].Item1;
                            titleCs = r[0].Item2;
                        }
                    }
                }

                List<string> genreListCs = await InitializeCheckboxesAsync(database: database);
                return new SongDto(genreListCs, songId, textCs, titleCs);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[IndexModel: OnPost Error]");
                return new SongDto() { ErrorMessageCs = "[IndexModel: OnPost Error]" };
            }
        }

        private async Task<List<string>> InitializeCheckboxesAsync(RsseContext database)
        {
            List<string> genreListCs = new List<string>();
            List<Tuple<string, int>> genreList = await database.ReadGenreListSql().ToListAsync();
            foreach (var r in genreList)
            {
                if (r.Item2 > 0)
                {
                    genreListCs.Add(r.Item1 + ": " + r.Item2);
                }
                else
                {
                    genreListCs.Add(r.Item1);
                }
            }
            return genreListCs;
        }
    }
}