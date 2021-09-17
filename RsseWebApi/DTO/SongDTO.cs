using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RandomSongSearchEngine.DTO
{
    public class SongDto : ISongModel
    {
        [JsonPropertyName("checkedCheckboxesJs")] public List<int> CheckedCheckboxesRequest { get; set; }

        [JsonPropertyName("titleJs")] public string TitleRequest { get; set; }

        [JsonPropertyName("textJs")] public string TextRequest { get; set; }

        [JsonPropertyName("savedTextId")] public int CurrentTextId { get; set; }

        [JsonPropertyName("textCS")] public string TextResponse { get; set; }

        [JsonPropertyName("titleCS")] public string TitleResponse { get; set; }

        [JsonPropertyName("isGenreCheckedCS")] public List<string> CheckedCheckboxesResponse { get; set; }

        [JsonPropertyName("genresNamesCS")] public List<string> GenreListResponse { get; set; }

        public string ErrorMessageResponse { get; set; }

        public SongDto()
        {
        }

        public SongDto(List<string> genreListCs, int savedTextId = 0, string textCs = "", string titleCs = "", List<string> checkedCheckboxesCs = null)
        {
            TextResponse = textCs;
            TitleResponse = titleCs;
            CheckedCheckboxesResponse = checkedCheckboxesCs ?? new List<string>();
            GenreListResponse = genreListCs;
            CurrentTextId = savedTextId;
            //CheckedCheckboxesJs = null;
            //TitleJs = "";
            //TextJs = "";
            //ErrorMessageCs = "";
        }
    }
}