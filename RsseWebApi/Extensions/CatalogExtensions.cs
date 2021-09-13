using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Models;
using System;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Extensions
{
    public static class CatalogModelExtensions
    {
        public static async Task OnGetCatalogAsync(this CatalogModel model, int id)
        {
            await model.GetSongCountAsync();//
            model.PageNumber = id;
            try
            {
                using var scope = model.ServiceScopeFactory.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<RsseContext>();//
                await model.GetSongsForCatalogAsync(database, model.PageNumber, model.PageSize);
            }
            catch (Exception e)
            {
                model.Logger.LogError(e, "[CatalogModel]");
            }
        }

        public static async Task OnPostCatalogAsync(this CatalogModel model)
        {
            await model.GetSongCountAsync();//
            //Предполагается, что браузер прислал неиспорченные данные
            if (model.NavigationButtons != null && model.NavigationButtons[0] == 2)
            {
                //double r = Math.Ceiling(SongsCount / (double)pageSize);
                int pageCount = Math.DivRem(model.SongsCount, model.PageSize, out int remainder);
                if (remainder > 0)
                {
                    pageCount++;
                }
                if (model.PageNumber < pageCount)
                {
                    model.PageNumber++;
                }
            }
            if (model.NavigationButtons != null && model.NavigationButtons[0] == 1)
            {
                if (model.PageNumber > 1)
                {
                    model.PageNumber--;
                }
            }
            await model.OnGetCatalogAsync(model.PageNumber);
        }

        /// <summary>
        /// Создание заголовков песен и их ID для CatalogModel
        /// </summary>
        /// <param name="model">Моделька</param>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="savedLastViewedPage">Текущая страница каталога</param>
        /// <param name="pageSize">Количество песен на странице</param>
        /// <returns></returns>
        public static async Task GetSongsForCatalogAsync(this CatalogModel model, RsseContext database, int savedLastViewedPage, int pageSize)
        {
            model.TitlesAndIds = await database.ReadSongsForCatalogSql(savedLastViewedPage, pageSize).ToListAsync();
            if (model.TitlesAndIds.Count > 0) model.SavedTextId = model.TitlesAndIds[0].Item2;
        }
    }
}
