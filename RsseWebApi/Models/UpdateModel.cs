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
        private ILogger<UpdateModel> _logger { get; }
        public UpdateModel(IServiceScope scope)
        {
            _scope = scope;
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<UpdateModel>>();
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
                return new SongDto() { ErrorMessageResponse = "[ChangeTextModel: OnGet Error]" };
            }
        }

        public async Task<SongDto> OnPostUpdateAsync(SongDto dto)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                if (dto.CheckedCheckboxesRequest == null || dto.TextRequest == null || dto.TitleRequest == null 
                    || dto.CheckedCheckboxesRequest.Count == 0 || dto.TextRequest == "" || dto.TitleRequest == "")
                {
                    return await OnGetUpdateAsync(dto.CurrentTextId);
                }
                List<int> originalGenres = await database.ReadSongGenresSql(dto.CurrentTextId).ToListAsync();
                await database.UpdateSongSqlAsync(originalGenres, new InnerDto(dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChangeTextModel: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[ChangeTextModel: OnPost Error]" };
            }
            return await OnGetUpdateAsync(dto.CurrentTextId);
        }

        public SongDto CreateDto(List<string> genreListCs, int savedTextId, string textCs, string titleCs, List<string> checkedCheckboxesCs)
        {
            var dto = new SongDto()
            {
                TextResponse = textCs,
                TitleResponse = titleCs,
                CheckedCheckboxesResponse = checkedCheckboxesCs,
                GenreListResponse = genreListCs,
                CurrentTextId = savedTextId
            };
            return dto;
        }
    }
}
