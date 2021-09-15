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
    public class UpdateModel
    {
        private IServiceScope _scope { get; }
        private ILogger<SongModel> _logger { get; }
        public UpdateModel(IServiceScope scope)
        {
            _scope = scope;
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<SongModel>>();
        }

        public async Task<SongDto> OnGetUpdateAsync(int textId)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                //if (SavedTextId == 0) throw new NotImplementedException();
                string textCs = "";
                string titleCs = "";
                List<string> genreListCs = new List<string>();//
                List<string> checkedCheckboxesCs = new List<string>();
                List<Tuple<string, int>> genreList = await database.ReadGenreListSql().ToListAsync();
                List<int> checkedList = await database.ReadSongGenresSql(textId).ToListAsync();
                var song = await database.ReadSongSql(textId).ToListAsync();
                if (song.Count > 0)
                {
                    textCs = song[0].Item1;
                    titleCs = song[0].Item2;
                }
                foreach (var genreAndAmount in genreList)
                {
                    genreListCs.Add(genreAndAmount.Item2 > 0 ? genreAndAmount.Item1 + ": " + genreAndAmount.Item2 : genreAndAmount.Item1);
                }
                for (int i = 0; i < genreListCs.Count; i++)
                {
                    checkedCheckboxesCs.Add("unchecked");
                }
                foreach (int i in checkedList)
                {
                    checkedCheckboxesCs[i - 1] = "checked";
                }
                return CreateDto(genreListCs, textId, textCs, titleCs, checkedCheckboxesCs);//
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChangeTextModel: OnGet Error]");
                return new SongDto() { ErrorMessageCs = "[ChangeTextModel: OnGet Error]" };
            }
        }

        public async Task<SongDto> OnPostUpdateAsync(SongDto dto)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                if (dto.CheckedCheckboxesJs == null || dto.TextJs == null || dto.TitleJs == null 
                    || dto.CheckedCheckboxesJs.Count == 0 || dto.TextJs == "" || dto.TitleJs == "")
                {
                    return await OnGetUpdateAsync(dto.SavedTextId);
                }
                List<int> originalGenres = await database.ReadSongGenresSql(dto.SavedTextId).ToListAsync();
                await database.UpdateSongSqlAsync(originalGenres, new InnerDto(dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChangeTextModel: OnPost Error]");
                return new SongDto() { ErrorMessageCs = "[ChangeTextModel: OnPost Error]" };
            }
            return await OnGetUpdateAsync(dto.SavedTextId);
        }

        public SongDto CreateDto(List<string> genreListCs, int savedTextId, string textCs, string titleCs, List<string> checkedCheckboxesCs)
        {
            var dto = new SongDto()
            {
                TextCs = textCs,
                TitleCs = titleCs,
                CheckedCheckboxesCs = checkedCheckboxesCs,
                GenreListCs = genreListCs,
                SavedTextId = savedTextId
            };
            return dto;
        }
    }
}
