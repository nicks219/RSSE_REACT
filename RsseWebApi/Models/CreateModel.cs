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
    public class CreateModel : DatabaseAccess
    {
        private IServiceScope _scope { get; }
        private ILogger<CreateModel> _logger { get; }

        public CreateModel(IServiceScope serviceScope)
        {
            _scope = serviceScope;
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<CreateModel>>();
        }

        public async Task<SongDto> OnGetAsync()
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                List<string> genreListResponse = await ReadGenreListAsync(database: database);
                return new SongDto(genreListResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateModel: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[CreateModel: OnGet Error]" };
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
                    SongDto errorDto = await OnGetAsync();
                    errorDto.ErrorMessageResponse = "[CreateModel: OnPost Error - empty data]";
                    if (dto.TextRequest != null && dto.TextRequest != "") errorDto.TextResponse = dto.TextRequest;
                    return errorDto;
                }

                int newSongId = await CreateSongAsync(database, dto);
                if (newSongId == 0)
                {
                    SongDto errorDto = await OnGetAsync();
                    errorDto.ErrorMessageResponse = "[CreateModel: OnPost Error - create unsuccessfull]";
                    errorDto.TitleResponse = "[Already Exist]";
                    return errorDto;
                }

                SongDto updatedDto = await OnGetAsync();
                List<string> updatedGenreList = updatedDto.GenreListResponse;
                List<string> songGenresResponse = new List<string>();
                for (int i = 0; i < updatedGenreList.Count; i++)
                {
                    songGenresResponse.Add("unchecked");
                }
                foreach (int i in dto.SongGenresRequest)
                {
                    songGenresResponse[i - 1] = "checked";
                }
                return new SongDto(updatedGenreList, newSongId, "", "[OK]", songGenresResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateModel: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[CreateModel: OnPost Error]" };
            }
        }
    }
}