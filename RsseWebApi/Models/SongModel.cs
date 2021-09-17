using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DTO;
using System.Collections.Generic;

namespace RandomSongSearchEngine.Models
{
    /// <summary>
    /// Базовый класс-помойка для моделей WebAPI
    /// </summary>
    public partial class SongModel : ISongModel
    {
        public IServiceScopeFactory ServiceScopeFactory { get; }
        public ILogger<SongModel> Logger { get; }

        //конструктор без параметров был нужен для работы десериализации (но в какой ситуации??)
        public SongModel() { }

        public SongModel(IServiceScopeFactory serviceScopeFactory)
        {
            CheckedCheckboxesResponse = new List<string>();
            GenreListResponse = new List<string>();
            ServiceScopeFactory = serviceScopeFactory;
            Logger = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<SongModel>>();
        }

        /// <summary>
        /// Заголовок песни из вьюхи
        /// </summary>
        public string TitleRequest { get; set; }

        /// <summary>
        /// Текст песни из вьюхи
        /// </summary>
        public string TextRequest { get; set; }

        //переименуй в checkboxesAreChecked
        /// <summary>
        /// Список выбраных чекбоксов из вьюхи
        /// </summary>
        public List<int> CheckedCheckboxesRequest { get; set; }

        /// <summary>
        /// Текст песни для вьюхи
        /// </summary>
        public string TextResponse { get; set; }

        /// <summary>
        /// Заголовок песни для вьюхи
        /// </summary>
        public string TitleResponse { get; set; }

        /// <summary>
        /// Список строк "checked" или "unchecked" для создания чекбоксов во вьюхе для Update
        /// </summary>
        public List<string> CheckedCheckboxesResponse { get; set; }

        /// <summary>
        /// Сохраненный ID текста для перехода между вьюхами
        /// </summary>
        public int CurrentTextId { get; set; }// = 3;

        /// <summary>
        /// Список с названиями жанров песен и количеством песен в каждом жанре
        /// </summary>
        public List<string> GenreListResponse { get; set; }

        /// <summary>
        /// Количество жанров песен, private
        /// </summary>
        private int GenresCount { get; set; }
    }
}