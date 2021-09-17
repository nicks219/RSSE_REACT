using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Models;
using System;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Extensions
{
    public static class CreateSongExtensions
    {
        public static async Task OnGetCreateAsync(this SongModel model)//CreateModel
        {
            try
            {
                using var scope = model.ServiceScopeFactory.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<RsseContext>();//
                await model.GetCheckboxesAsync(database);
            }
            catch (Exception e)
            {
                model.Logger.LogError(e, "[AddTextModel]");
            }
        }

        public static async Task OnPostCreateAsync(this SongModel model)
        {
            if (model.CheckedCheckboxesRequest == null || model.TextRequest == null || model.TitleRequest == null || model.CheckedCheckboxesRequest.Count == 0
                || model.TextRequest == "" || model.TitleRequest == "")
            {
                //очищаем модель, иначе список чекбоксов пустой останется
                await model.OnGetCreateAsync();
                return;
            }
            try
            {
                using (var scope = model.ServiceScopeFactory.CreateScope())//
                {
                    var database = scope.ServiceProvider.GetRequiredService<RsseContext>();//

                    await model.CreateSongAsync(database);
                    await model.GetSongAsync(database, model.CurrentTextId);
                }
                await model.OnGetCreateAsync();//
                model.SetChecked();
            }
            catch (Exception e)
            {
                model.Logger.LogError(e, "[AddTextModel]");
                await model.OnGetCreateAsync();//
            }
        }

        /// <summary>
        /// Добавление песни
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="database">Контекст базы данных</param>
        /// <returns></returns>
        public static async Task CreateSongAsync(this SongModel model, RsseContext database)
        {
            InnerDto dt = new InnerDto
            {
                TitleFromHtml = model.TitleRequest.Trim(),
                TextFromHtml = model.TextRequest.Trim(),
                AreChecked = model.CheckedCheckboxesRequest
            };
            //if (ModelState.IsValid)
            {
                //Получим 0 при ошибке
                model.CurrentTextId = await database.CreateSongSqlAsync(dt);
            }
        }
    }
}
