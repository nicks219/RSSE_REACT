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
            model.SavedTextId = id;
            try
            {
                if (model.SavedTextId == 0)
                {
                    //не помню, в каком случае появляется ошибка
                    throw new NotImplementedException();
                }

                using var scope = model.ServiceScopeFactory.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
                await model.GetSongAsync(database, model.SavedTextId);
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
                if (model.CheckedCheckboxesJs == null || model.TextJs == null || model.TitleJs == null || model.CheckedCheckboxesJs.Count == 0
                    || model.TextJs == "" || model.TitleJs == "")
                {
                    await model.OnGetUpdateAsync(model.SavedTextId);
                    return;
                }

                using var scope = model.ServiceScopeFactory.CreateScope();
                var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
                //List<int> initialCheckboxes = IGlobalData.InitialCheckboxes;
                //повторное чтение из бд жанров песни - чтобы избавиться от состояния и передачи этих данных через dto
                List<int> initialCheckboxes = await database.ReadGenresForUpdateSql(model.SavedTextId).ToListAsync();
                await model.UpdateSongAsync(database, initialCheckboxes);
            }
            catch (Exception e)
            {
                model.Logger.LogError(e, "[ChangeTextModel]");
            }
            await model.OnGetUpdateAsync(model.SavedTextId);
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
            List<int> checkedList = await database.ReadGenresForUpdateSql(model.SavedTextId).ToListAsync();
            foreach (int i in checkedList)
            {
                model.CheckedCheckboxesCs[i - 1] = "checked";
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
                TitleFromHtml = model.TitleJs.Trim(),
                TextFromHtml = model.TextJs.Trim(),
                AreChecked = model.CheckedCheckboxesJs,
                SavedTextId = model.SavedTextId
            };
            //тут можно проверить имя на свопадение с существующим. редкая ошибка
            //if (ModelState.IsValid)
            {
                await database.UpdateSongSqlAsync(initialCheckboxes, dt);
            }
        }
    }
}