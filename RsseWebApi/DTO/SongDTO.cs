using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RandomSongSearchEngine.DTO
{
    public struct SongDto : ISongModel
    {
        public List<int> CheckedCheckboxesJs { get; set; }

        public string TitleJs { get; set; }

        public string TextJs { get; set; }

        public int SavedTextId { get; set; }

        [JsonPropertyName("textCS")]
        public string TextCs { get; set; }

        [JsonPropertyName("titleCS")]
        public string TitleCs { get; set; }

        [JsonPropertyName("isGenreCheckedCS")]
        public List<string> IsGenreCheckedCs { get; set; }

        [JsonPropertyName("genresNamesCS")]
        public List<string> GenresNamesCs { get; set; }
    }
}