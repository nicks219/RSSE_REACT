using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Extensions
{
    public static class UpdateSongExtensinos
    {
        public static async Task OnGetUpdateAsync(this SongModel model, int id)
        {
            model.CurrentTextId = id;
            try
            {
                if (model.CurrentTextId == 0)
                {
                    //не помню, в каком случае появляется ошибка
                    throw new NotImplementedException();
                }

                using var scope = model.ServiceScopeFactory.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
                await model.GetSongAsync(database, model.CurrentTextId);
                await model.GetCheckboxesAsync(database);
                //IGlobalData.InitialCheckboxes = await model.GetGenresForSongAsync(database);
                await model.GetGenresForSongAsync(database);
            }
            catch (Exception e)
            {
                model.Logger.LogError(e, "[ChangeTextModel]");
            }
        }

        public static async Task OnPostUpdateAsync(this SongModel model)
        {
            try
            {
                if (model.CheckedCheckboxesRequest == null || model.TextRequest == null || model.TitleRequest == null || model.CheckedCheckboxesRequest.Count == 0
                    || model.TextRequest == "" || model.TitleRequest == "")
                {
                    await model.OnGetUpdateAsync(model.CurrentTextId);
                    return;
                }

                using var scope = model.ServiceScopeFactory.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
                //List<int> initialCheckboxes = IGlobalData.InitialCheckboxes;
                //повторное чтение из бд жанров песни - чтобы избавиться от состояния и передачи этих данных через dto
                List<int> initialCheckboxes = await database.ReadSongGenresSql(model.CurrentTextId).ToListAsync();
                await model.UpdateSongAsync(database, initialCheckboxes);
            }
            catch (Exception e)
            {
                model.Logger.LogError(e, "[ChangeTextModel]");
            }
            await model.OnGetUpdateAsync(model.CurrentTextId);
        }

        /// <summary>
        /// Формируется список жанров, к которым принадлежит песня
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="database">Контекст базы данных</param>
        /// <returns>Список жанров</returns>
        private static async Task GetGenresForSongAsync(this SongModel model, RsseContext database)
        {
            //read checked genres for song
            List<int> checkedList = await database.ReadSongGenresSql(model.CurrentTextId).ToListAsync();
            foreach (int i in checkedList)
            {
                model.CheckedCheckboxesResponse[i - 1] = "checked";
            }
        }

        /// <summary>
        /// Изменение песни
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="initialCheckboxes">Новый список отмеченных во вьюхе жанров</param>
        /// <returns></returns>
        public static async Task UpdateSongAsync(this SongModel model, RsseContext database, List<int> initialCheckboxes)
        {
            InnerDto dt = new InnerDto
            {
                TitleFromHtml = model.TitleRequest.Trim(),
                TextFromHtml = model.TextRequest.Trim(),
                AreChecked = model.CheckedCheckboxesRequest,
                SavedTextId = model.CurrentTextId
            };
            //тут можно проверить имя на свопадение с существующим. редкая ошибка
            //if (ModelState.IsValid)
            {
                await database.UpdateSongSqlAsync(initialCheckboxes, dt);
            }
        }
    }
}