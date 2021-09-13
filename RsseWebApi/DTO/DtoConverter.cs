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
                IsGenreCheckedCs = model.IsGenreCheckedCs,
                GenresNamesCs = model.GenresNamesCs,
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

        public static CatalogDto CatalogToDto(this ICatalogModel model)
        {
            //закомментированы ненужные на данный момент поля
            var dto = new CatalogDto
            {
                //NavigationButtons = model.NavigationButtons,
                //SavedTextId = model.SavedTextId,
                PageNumber = model.PageNumber,
                TitlesAndIds = model.TitlesAndIds,
                SongsCount = model.SongsCount
            };
            return dto;
        }

        public static void DtoToCatalog(this ICatalogModel model, CatalogDto dto)
        {
            //закомментированы ненужные на данный момент поля
            model.NavigationButtons = dto.NavigationButtons;
            model.SavedTextId = dto.SavedTextId;
            model.PageNumber = dto.PageNumber;
            //model.SongsCount = dto.SongsCount;
            //model.TitlesAndIds = dto.TitlesAndIds;
        }
    }
}
