using System;
using System.Collections.Generic;

namespace RandomSongSearchEngine.DTO
{
    public class CatalogDto : ICatalogModel
    {
        public int SavedTextId { get; set; }

        public List<Tuple<string, int>> TitlesAndIds { get; set; }

        public List<int> NavigationButtons { get; set; }

        public int SongsCount { get; set; }

        public int PageNumber { get; set; }
    }
}