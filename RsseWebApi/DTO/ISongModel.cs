using System.Collections.Generic;

namespace RandomSongSearchEngine.DTO
{
    public interface ISongModel
    {
        //[JsonProperty("TitleCS")]
        //[JsonProperty(PropertyName = "FooBar")]
        //[JsonPropertyName("CheckedCheckboxesJS")]
        public List<int> CheckedCheckboxesJs { get; set; }

        public string TitleJs { get; set; }

        public string TextJs { get; set; }

        public int SavedTextId { get; set; }

        public string TextCs { get; set; }

        public string TitleCs { get; set; }

        public List<string> CheckedCheckboxesCs { get; set; }

        public List<string> GenreListCs { get; set; }
    }
}
