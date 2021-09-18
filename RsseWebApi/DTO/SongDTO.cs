using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RandomSongSearchEngine.Dto
{
    public class SongDto
    {
        // request
        [JsonPropertyName("checkedCheckboxesJs")] public List<int> SongGenres { get; set; }

        [JsonPropertyName("titleJs")] public string Title { get; set; }

        [JsonPropertyName("textJs")] public string Text { get; set; }

        // resopnse
        [JsonPropertyName("textCS")] public string TextResponse { get; set; }

        [JsonPropertyName("titleCS")] public string TitleResponse { get; set; }

        [JsonPropertyName("isGenreCheckedCS")] public List<string> SongGenresResponse { get; set; }

        [JsonPropertyName("genresNamesCS")] public List<string> GenreListResponse { get; set; }

        public string ErrorMessageResponse { get; set; }

        // request and response
        [JsonPropertyName("savedTextId")] public int Id { get; set; }

        public SongDto()
        {
        }

        public SongDto(List<string> genreListCs, int savedTextId = 0, string textCs = "", string titleCs = "", List<string> checkedCheckboxesCs = null)
        {
            TextResponse = textCs;
            TitleResponse = titleCs;
            SongGenresResponse = checkedCheckboxesCs ?? new List<string>();
            GenreListResponse = genreListCs;
            Id = savedTextId;
        }
    }
}