using Microsoft.EntityFrameworkCore;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    public partial class SongModel
    {
        /// <summary>
        /// Изменение отмеченных жанров в списке на "checked"
        /// </summary>
        public void SetChecked()
        {
            if (CheckedCheckboxesJs == null) return;
            foreach (int i in CheckedCheckboxesJs)
            {
                CheckedCheckboxesCs[i - 1] = "checked";
            }
        }

        /// <summary>
        /// Создание заголовка и текста песни
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="textId">ID песни</param>
        /// <returns></returns>
        public async Task GetSongAsync(RsseContext database, int textId)
        {
            //метод оставляет на экране ранее введенный текст в случае большинства исключений
            var r = await database.ReadSongSql(textId).ToListAsync();
            if (r.Count > 0)
            {
                TextCs = r[0].Item1;
                TitleCs = r[0].Item2;
            }
            else
            {
                TextCs = TextJs;
                TitleCs = TitleJs;
            }
        }

        /// <summary>
        /// Создание списка жанров с количеством песен для чекбоксов
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <returns></returns>
        public async Task GetCheckboxesAsync(RsseContext database)
        {
            List<Tuple<string, int>> genresNames = await database.ReadGenreListSql().ToListAsync();

            GenreListCs = new List<string>();//

            foreach (var r in genresNames)
            {
                if (r.Item2 > 0)
                {
                    GenreListCs.Add(r.Item1 + ": " + r.Item2);
                }
                else
                {
                    GenreListCs.Add(r.Item1);
                }
            }
            GenresCount = GenreListCs.Count;
            SetUnchecked();
        }

        /// <summary>
        /// Инициализация списка жанров с пометкой "unchecked"
        /// </summary>
        protected void SetUnchecked()
        {

            CheckedCheckboxesCs = new List<string>();//

            for (int i = 0; i < GenresCount; i++)
            {
                CheckedCheckboxesCs.Add("unchecked");
            }
        }
    }
}
