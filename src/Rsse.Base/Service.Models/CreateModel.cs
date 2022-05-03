using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Data.Repository.Contracts;

namespace RandomSongSearchEngine.Service.Models;

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
        await using var repo = _scope.ServiceProvider.GetRequiredService<IRepository>();
        try
        {
            List<string> genreListResponse = await repo.ReadGenreListAsync();
            return new SongDto(genreListResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CreateModel: OnGet Error]");
            return new SongDto() {ErrorMessageResponse = "[CreateModel: OnGet Error]"};
        }
    }

    public async Task<SongDto> CreateSongAsync(SongDto createdSong)
    {
        await using var repo = _scope.ServiceProvider.GetRequiredService<IRepository>();
        try
        {
            if (createdSong.SongGenres == null || string.IsNullOrEmpty(createdSong.Text)
                                               || string.IsNullOrEmpty(createdSong.Title) ||
                                               createdSong.SongGenres.Count == 0)
            {
                SongDto errorDto = await ReadGenreListAsync();
                errorDto.ErrorMessageResponse = "[CreateModel: OnPost Error - empty data]";
                if (!string.IsNullOrEmpty(createdSong.Text)) errorDto.TextResponse = createdSong.Text;
                return errorDto;
            }

            int newSongId = await repo.CreateSongAsync(createdSong);
            if (newSongId == 0)
            {
                SongDto errorDto = await ReadGenreListAsync();
                errorDto.ErrorMessageResponse = "[CreateModel: OnPost Error - create unsuccessfull]";
                errorDto.TitleResponse = "[Already Exist]";
                return errorDto;
            }

            SongDto updatedDto = await ReadGenreListAsync();
            List<string> updatedGenreList = updatedDto.GenreListResponse!;
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
            return new SongDto() {ErrorMessageResponse = "[CreateModel: OnPost Error]"};
        }
    }
}