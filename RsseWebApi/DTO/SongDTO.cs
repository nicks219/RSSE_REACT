using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RandomSongSearchEngine.DTO
{
    public class SongDto : ISongModel
    {
        public List<int> CheckedCheckboxesJs { get; set; }

        public string TitleJs { get; set; }

        public string TextJs { get; set; }

        public int SavedTextId { get; set; }

        [JsonPropertyName("textCS")] public string TextCs { get; set; }

        [JsonPropertyName("titleCS")] public string TitleCs { get; set; }

        [JsonPropertyName("isGenreCheckedCS")] public List<string> CheckedCheckboxesCs { get; set; }

        [JsonPropertyName("genresNamesCS")] public List<string> GenreListCs { get; set; }

        public string ErrorMessageCs { get; set; }

        public SongDto()
        {
        }

        // конструктор для ReadModel
        public SongDto(List<string> genreListCs, int savedTextId = 0, string textCs = "", string titleCs = "", List<string> checkedCheckboxesCs = null)
        {
            TextCs = textCs;
            TitleCs = titleCs;
            CheckedCheckboxesCs = checkedCheckboxesCs;
            GenreListCs = genreListCs;
            SavedTextId = savedTextId;
            //???
            //CheckedCheckboxesJs = null;
            //TitleJs = "";
            //TextJs = "";
            //ErrorMessageCs = "";
        }

    }
}