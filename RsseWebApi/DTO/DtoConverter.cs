namespace RandomSongSearchEngine.DTO
{
    public static class DtoConverter
    {
        public static SongDto ModelToDto(this ISongModel model)
        {
            //поля JS фронту не нужны
            var dto = new SongDto
            {
                //CheckedCheckboxesJS = model.CheckedCheckboxesJS,
                //TextJS = model.TextJS,
                //TitleJS = model.TitleJS,
                TextCs = model.TextCs,
                TitleCs = model.TitleCs,
                CheckedCheckboxesCs = model.CheckedCheckboxesCs,
                GenreListCs = model.GenreListCs,
                SavedTextId = model.SavedTextId
            };
            return dto;
        }

        public static void DtoToModel(this ISongModel model, SongDto dto)
        {
            //поля CS бэку не нужны
            model.CheckedCheckboxesJs = dto.CheckedCheckboxesJs;
            model.TextJs = dto.TextJs;
            model.TitleJs = dto.TitleJs;
            //model.TextCS = dto.TextCS;
            //model.TitleCS = dto.TitleCS;
            //model.IsGenreCheckedCS = dto.IsGenreCheckedCS;
            //model.GenresNamesCS = dto.GenresNamesCS;
            model.SavedTextId = dto.SavedTextId;
        }
    }
}
