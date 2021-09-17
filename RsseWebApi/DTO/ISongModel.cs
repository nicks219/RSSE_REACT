using System.Collections.Generic;

namespace RandomSongSearchEngine.DTO
{
    public interface ISongModel
    {
        //[JsonProperty("TitleCS")]
        //[JsonProperty(PropertyName = "FooBar")]
        //[JsonPropertyName("CheckedCheckboxesJS")]
        public List<int> CheckedCheckboxesRequest { get; set; }

        public string TitleRequest { get; set; }

        public string TextRequest { get; set; }

        public int CurrentTextId { get; set; }

        public string TextResponse { get; set; }

        public string TitleResponse { get; set; }

        public List<string> CheckedCheckboxesResponse { get; set; }

        public List<string> GenreListResponse { get; set; }
    }
}
