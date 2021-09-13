using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.DTO;

namespace RandomSongSearchEngine.Models
{
    /// <summary>
    /// Каталог песен
    /// </summary>
    public class CatalogModel : ICatalogModel
    {
        public IServiceScopeFactory ServiceScopeFactory { get; }
        public ILogger<CatalogModel> Logger { get; }

        //конструктор без параметров нужен для работы десериализации
        public CatalogModel(){}

        public CatalogModel(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory;
            Logger = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<CatalogModel>>();
            //модель должна знать songscount - иначе придется брать из фронта или считать каждый раз
            //количество песен меняется!
            _ = GetSongCount();
        }

        //у ParentModel тоже есть
        /// <summary>
        /// Сохраненный ID текста для перехода между вьюхами
        /// </summary>
        public int SavedTextId { get; set; }

        /// <summary>
        /// Список из названий песен и соответствующим им ID для CatalogView
        /// </summary>
        public List<Tuple<string, int>> TitlesAndIds { get; set; }

        /// <summary>
        /// Используется для определения какая кнопка была нажата во вьюхе
        /// </summary>
        public List<int> NavigationButtons { get; set; }

        /// <summary>
        /// Количество песен в базе данных
        /// </summary>
        public int SongsCount { get; set; }

        /// <summary>
        /// Номер последней просмотренной страницы
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Количество песен на одной странице
        /// </summary>
        public readonly int PageSize = 10;

        public async Task GetSongCount()
        {
            await GetSongCountAsync();
        }

        private async Task GetSongCountAsync()
        {
            try
            {
                using var scope = ServiceScopeFactory.CreateScope();//
                var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
                SongsCount = await database.Text.CountAsync();//
            }
            catch (Exception e)
            {
                Logger.LogError(e, "[CatalogModel]: no database");
                //в случае отсутствия бд мы не придём к null referenece exception из-за TitleAndTextID
                TitlesAndIds = new List<Tuple<string, int>>();
            }
        }
    }
}
