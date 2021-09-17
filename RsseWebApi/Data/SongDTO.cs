using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RandomSongSearchEngine.Data
{
    public class SongDto
    {
        [JsonPropertyName("checkedCheckboxesJs")] public List<int> SongGenresRequest { get; set; }

        [JsonPropertyName("titleJs")] public string TitleRequest { get; set; }

        [JsonPropertyName("textJs")] public string TextRequest { get; set; }

        [JsonPropertyName("savedTextId")] public int SongId { get; set; }

        [JsonPropertyName("textCS")] public string TextResponse { get; set; }

        [JsonPropertyName("titleCS")] public string TitleResponse { get; set; }

        [JsonPropertyName("isGenreCheckedCS")] public List<string> SongGenresResponse { get; set; }

        [JsonPropertyName("genresNamesCS")] public List<string> GenreListResponse { get; set; }

        public string ErrorMessageResponse { get; set; }

        public SongDto()
        {
        }

        public SongDto(List<string> genreListCs, int savedTextId = 0, string textCs = "", string titleCs = "", List<string> checkedCheckboxesCs = null)
        {
            TextResponse = textCs;
            TitleResponse = titleCs;
            SongGenresResponse = checkedCheckboxesCs ?? new List<string>();
            GenreListResponse = genreListCs;
            SongId = savedTextId;
        }
    }
}