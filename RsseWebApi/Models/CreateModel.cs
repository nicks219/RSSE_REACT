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
    public class CreateModel
    {
        private IServiceScope _scope { get; }
        private ILogger<SongModel> _logger { get; }

        public CreateModel(IServiceScope serviceScope)
        {
            _scope = serviceScope;
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<SongModel>>();
        }

        public async Task<SongDto> OnGetCreateAsync()
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                List<Tuple<string, int>> genreList = await database.ReadGenreListSql().ToListAsync();
                List<string> genreListResponse = new List<string>();
                foreach (var genreAndAmount in genreList)
                {
                    genreListResponse.Add(genreAndAmount.Item2 > 0 ? genreAndAmount.Item1 + ": " + genreAndAmount.Item2 : genreAndAmount.Item1);
                }
                return new SongDto(genreListResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateModel: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[CreateModel: OnGet Error]" };
            }
        }

        public async Task<SongDto> OnPostCreateAsync(SongDto dto)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                List<string> checkedCheckboxesResponse = new List<string>();
                if (dto.CheckedCheckboxesRequest == null || dto.TextRequest == null || dto.TitleRequest == null
                         || dto.CheckedCheckboxesRequest.Count == 0 || dto.TextRequest == "" || dto.TitleRequest == "")
                {
                    SongDto errorDto = await OnGetCreateAsync();
                    errorDto.ErrorMessageResponse = "[CreateModel: OnPost Error - empty data]";
                    if (dto.TextRequest != null && dto.TextRequest != "") errorDto.TextResponse = dto.TextRequest;
                    return errorDto;
                }

                int newTextId = await database.CreateSongSqlAsync(new InnerDto(dto));
                if (newTextId == 0)
                {
                    SongDto errorDto = await OnGetCreateAsync();
                    errorDto.ErrorMessageResponse = "[CreateModel: OnPost Error - create unsuccessfull]";
                    errorDto.TitleResponse = "[Already Exist]";
                    return errorDto;
                }

                SongDto updatedDto = await OnGetCreateAsync();
                List<string> updatedGenreList = updatedDto.GenreListResponse;
                for (int i = 0; i < updatedGenreList.Count; i++)
                {
                    checkedCheckboxesResponse.Add("unchecked");
                }
                foreach (int i in dto.CheckedCheckboxesRequest) // checkedList)
                {
                    checkedCheckboxesResponse[i - 1] = "checked";
                }
                return new SongDto(updatedGenreList, newTextId, "", "[OK]", checkedCheckboxesResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateModel: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[CreateModel: OnPost Error]" };
            }
        }
    }
}