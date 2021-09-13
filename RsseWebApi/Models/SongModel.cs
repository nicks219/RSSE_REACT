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
        public IServiceScopeFactory ServiceScopeFactory { get; set; }
        public ILogger<SongModel> Logger { get; set; }

        //конструктор без параметров нужен для работы десериализации
        //в базовом классе - чтоб работали конструкторы без параметров у наследников
        //public ParentModel() { }

        public SongModel(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            IsGenreCheckedCs = new List<string>();
            GenresNamesCs = new List<string>();
            ServiceScopeFactory = serviceScopeFactory;
            Logger = logger;
        }

        /// <summary>
        /// Заголовок песни из вьюхи
        /// </summary>
        public string TitleJs { get; set; }

        /// <summary>
        /// Текст песни из вьюхи
        /// </summary>
        public string TextJs { get; set; }

        //переименуй в checkboxesAreChecked
        /// <summary>
        /// Список выбраных чекбоксов из вьюхи
        /// </summary>
        public List<int> CheckedCheckboxesJs { get; set; }

        /// <summary>
        /// Текст песни для вьюхи
        /// </summary>
        public string TextCs { get; set; }

        /// <summary>
        /// Заголовок песни для вьюхи
        /// </summary>
        public string TitleCs { get; set; }

        /// <summary>
        /// Список строк "checked" или "unchecked" для создания чекбоксов во вьюхе для Update
        /// </summary>
        public List<string> IsGenreCheckedCs { get; set; }

        /// <summary>
        /// Сохраненный ID текста для перехода между вьюхами
        /// </summary>
        public int SavedTextId { get; set; }// = 3;

        /// <summary>
        /// Список с названиями жанров песен и количеством песен в каждом жанре
        /// </summary>
        public List<string> GenresNamesCs { get; set; }

        /// <summary>
        /// Количество жанров песен, private
        /// </summary>
        private int GenresCount { get; set; }
    }
}