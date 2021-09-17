using RandomSongSearchEngine.Models;

namespace RandomSongSearchEngine.DTO
{
    public static class DtoConverter
    {
        public static SongDto ModelToDto(this ISongModel model)
        {
            //поля JS фронту не нужны
            var dto = new SongDto()
            {
                //CheckedCheckboxesJS = model.CheckedCheckboxesJS,
                //TextJS = model.TextJS,
                //TitleJS = model.TitleJS,
                TextResponse = model.TextResponse,
                TitleResponse = model.TitleResponse,
                CheckedCheckboxesResponse = model.CheckedCheckboxesResponse,
                GenreListResponse = model.GenreListResponse,
                CurrentTextId = model.CurrentTextId
            };
            return dto;
        }

        public static void DtoToModel(this ISongModel model, SongDto dto)
        {
            //поля CS бэку не нужны
            model.CheckedCheckboxesRequest = dto.CheckedCheckboxesRequest;
            model.TextRequest = dto.TextRequest;
            model.TitleRequest = dto.TitleRequest;
            model.CurrentTextId = dto.CurrentTextId;
            //model.TextCS = dto.TextCS;
            //model.TitleCS = dto.TitleCS;
            //model.IsGenreCheckedCS = dto.IsGenreCheckedCS;
            //model.GenresNamesCS = dto.GenresNamesCS;

        }
    }
}
