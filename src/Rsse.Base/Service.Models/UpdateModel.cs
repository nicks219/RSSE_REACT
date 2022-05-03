﻿using Microsoft.EntityFrameworkCore;
using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Data.Repository.Contracts;

namespace RandomSongSearchEngine.Service.Models;

public class UpdateModel
{
    private IServiceScope _scope { get; }
    private ILogger<UpdateModel> _logger { get; }

    public UpdateModel(IServiceScope scope)
    {
        _scope = scope;
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<UpdateModel>>();
    }

    public async Task<SongDto> ReadOriginalSongAsync(int originalSongId)
    {
        await using var repo = _scope.ServiceProvider.GetRequiredService<IRepository>();
        try
        {
            //if (SavedTextId == 0) throw new NotImplementedException();
            string textResponse = "";
            string titleResponse = "";
            List<Tuple<string, string>> song = await repo.ReadSong(originalSongId).ToListAsync();
            if (song.Count > 0)
            {
                textResponse = song[0].Item1;
                titleResponse = song[0].Item2;
            }
            else
            {
                textResponse = "[Song: Always Deleted. Select another pls]";
            }

            List<string> genreListResponse = await repo.ReadGenreListAsync();
            List<int> songGenres = await repo.ReadSongGenres(originalSongId).ToListAsync();
            List<string> songGenresResponse = new List<string>();
            for (int i = 0; i < genreListResponse.Count; i++)
            {
                songGenresResponse.Add("unchecked");
            }

            foreach (int i in songGenres)
            {
                songGenresResponse[i - 1] = "checked";
            }

            return new SongDto(genreListResponse, originalSongId, textResponse, titleResponse, songGenresResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChangeTextModel: OnGet Error]");
            return new SongDto() {ErrorMessageResponse = "[ChangeTextModel: OnGet Error]"};
        }
    }

    public async Task<SongDto> UpdateSongAsync(SongDto updatedSong)
    {
        await using var repo = _scope.ServiceProvider.GetRequiredService<IRepository>();
        try
        {
            if (updatedSong.SongGenres == null || string.IsNullOrEmpty(updatedSong.Text)
                                               || string.IsNullOrEmpty(updatedSong.Title) ||
                                               updatedSong.SongGenres.Count == 0)
            {
                return await ReadOriginalSongAsync(updatedSong.Id);
            }

            List<int> originalGenres = await repo.ReadSongGenres(updatedSong.Id).ToListAsync();
            await repo.UpdateSongAsync(originalGenres, updatedSong);
            return await ReadOriginalSongAsync(updatedSong.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChangeTextModel: OnPost Error]");
            return new SongDto() {ErrorMessageResponse = "[ChangeTextModel: OnPost Error]"};
        }
    }
}
