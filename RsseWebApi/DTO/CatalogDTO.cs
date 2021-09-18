using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RandomSongSearchEngine.Dto
{
    public class CatalogDto
    {
        //название и соответствующее ему Id из бд
        [JsonPropertyName("titlesAndIds")] public List<Tuple<string, int>> CatalogPage { get; set; }

        [JsonPropertyName("navigationButtons")] public List<int> NavigationButtons { get; set; }

        [JsonPropertyName("songsCount")] public int SongsCount { get; set; }

        [JsonPropertyName("pageNumber")] public int PageNumber { get; set; }

        [JsonPropertyName("errorMessage")] public string ErrorMessage { get; internal set; }

        public int Direction()
        {
            if (NavigationButtons is null) return 0;
            if (NavigationButtons[0] == 1) return 1;
            if (NavigationButtons[0] == 2) return 2;
            throw new NotImplementedException("[Wrong Navigate]");
        }
    }
}