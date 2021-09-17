using RandomSongSearchEngine.Models;
using System.Collections.Generic;

namespace RandomSongSearchEngine.DTO
{
    /// <summary>
    /// Структура для передачи данных песни
    /// </summary>
    public struct InnerDto
    {
        /// <summary>
        /// Название песни из вьюхи
        /// </summary>
        public string TitleFromHtml { get; set; }

        /// <summary>
        /// Текст песни из вьхи
        /// </summary>
        public string TextFromHtml { get; set; }

        /// <summary>
        /// Жанры песни из вьюхи
        /// </summary>
        public List<int> AreChecked { get; set; }

        /// <summary>
        /// ID песни для передачи между страницами приложения
        /// </summary>
        public int SavedTextId { get; set; }

        public InnerDto(SongDto model)
        {
            TitleFromHtml = model.TitleRequest.Trim();
            TextFromHtml = model.TextRequest.Trim();
            AreChecked = model.CheckedCheckboxesRequest;
            SavedTextId = model.CurrentTextId;
        }
    }
}
