using System;
using System.Collections.Generic;

namespace RandomSongSearchEngine.DTO
{
    public class CatalogDto // : ICatalogModel
    {
        /// <summary>
        /// Список из названий песен и соответствующим им Id для бд
        /// </summary>
        public List<Tuple<string, int>> TitlesAndIds { get; set; }

        /// <summary>
        /// Используется для определения какая кнопка была нажата во вьюхе
        /// </summary>
        public List<int> NavigationButtons { get; set; }

        //TODO: считай их количество на фронте
        /// <summary>
        /// Количество песен в базе данных
        /// </summary>
        public int SongsCount { get; set; }

        /// <summary>
        /// Номер последней просмотренной страницы
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Поле для сообщения об ошибке
        /// </summary>
        public string ErrorMessage { get; internal set; }
    }
}