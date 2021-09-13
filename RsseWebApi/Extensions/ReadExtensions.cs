using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Models;
using System;
using System.Threading.Tasks;
using RandomSongSearchEngine.Services.Services;

namespace RandomSongSearchEngine.Extensions
{
    public static class IndexModelExtensions
    {
        public static async Task OnGetReadAsync(this SongModel model)
        {
            try
            {
                using (var scope = model.ServiceScopeFactory.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
                    await model.GetCheckboxesAsync(database: database);
                }
                model.SetChecked();
            }
            catch (Exception e)
            {
                model.Logger.LogError(e, "[IndexModel: OnGet Error]");
            }
        }

        public static async Task OnPostReadAsync(this SongModel model)
        {
            try
            {
                await model.ReadRandomSongAsync();
                await model.OnGetReadAsync();
            }
            catch (Exception e)
            {
                model.Logger.LogError(e, "[IndexModel: OnPost Error]");
            }
        }

        /// <summary>
        /// Выбирает из базы данных случайную песню, относящуюся к заданным категориям
        /// </summary>
        private static async Task ReadRandomSongAsync(this SongModel model)
        {
            if (model.CheckedCheckboxesJs == null || model.CheckedCheckboxesJs.Count == 0)
            {
                return;
            }

            using var scope = model.ServiceScopeFactory.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
            int randomResult = await database.GetRandomSongAsync(model.CheckedCheckboxesJs);
            if (randomResult == 0)
            {
                return;
            }
            await model.GetSongAsync(database, randomResult);
            model.SavedTextId = randomResult;
        }
    }
}
