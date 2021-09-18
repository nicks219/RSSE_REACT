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
    public class CreateModel
    {
        private IServiceScope _scope { get; }
        private ILogger<CreateModel> _logger { get; }

        public CreateModel(IServiceScope serviceScope)
        {
            _scope = serviceScope;
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<CreateModel>>();
        }

        public async Task<SongDto> ReadGenreListAsync()
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<IDatabaseAccess>();
            try
            {
                List<string> genreListResponse = await database.ReadGenreListAsync();
                return new SongDto(genreListResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateModel: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[CreateModel: OnGet Error]" };
            }
        }

        public async Task<SongDto> CreateSongAsync(SongDto createdSong)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<IDatabaseAccess>();
            try
            {
                if (createdSong.SongGenres == null || createdSong.Text == null || createdSong.Title == null
                         || createdSong.SongGenres.Count == 0 || createdSong.Text == "" || createdSong.Title == "")
                {
                    SongDto errorDto = await ReadGenreListAsync();
                    errorDto.ErrorMessageResponse = "[CreateModel: OnPost Error - empty data]";
                    if (!string.IsNullOrEmpty(createdSong.Text)) errorDto.TextResponse = createdSong.Text;
                    return errorDto;
                }

                int newSongId = await database.CreateSongAsync(createdSong);
                if (newSongId == 0)
                {
                    SongDto errorDto = await ReadGenreListAsync();
                    errorDto.ErrorMessageResponse = "[CreateModel: OnPost Error - create unsuccessfull]";
                    errorDto.TitleResponse = "[Already Exist]";
                    return errorDto;
                }

                SongDto updatedDto = await ReadGenreListAsync();
                List<string> updatedGenreList = updatedDto.GenreListResponse;
                List<string> songGenresResponse = new List<string>();
                for (int i = 0; i < updatedGenreList.Count; i++)
                {
                    songGenresResponse.Add("unchecked");
                }
                foreach (int i in createdSong.SongGenres)
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