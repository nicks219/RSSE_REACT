using Microsoft.EntityFrameworkCore;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    public interface IModel { }

    public class TempModelExtensions : SongModel
    {
        /// <summary>
        /// Создание заголовка и текста песни
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="textId">ID песни</param>
        /// <returns></returns>
        public async Task<(string textCs, string titleCs)> GetSongAsync(RsseContext database, int textId, SongDto dto)
        {
            string textCs;
            string titleCs;
            //метод оставляет на экране ранее введенный текст в случае большинства исключений
            var r = await database.ReadSongSql(textId).ToListAsync();
            if (r.Count > 0)
            {
                textCs = r[0].Item1;
                titleCs = r[0].Item2;
            }
            else
            {
                textCs = dto.TextJs;
                titleCs = dto.TitleJs;
            }
            return (textCs, titleCs);
        }

        /// <summary>
        /// Создание списка жанров с количеством песен для чекбоксов
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <returns></returns>
        public async Task<List<string>> InitCheckboxesAsync(RsseContext database)
        {
            List<Tuple<string, int>> genreList = await database.ReadGenreListSql().ToListAsync();
            //List<string> checkedCheckboxesCs = new List<string>();
            List<string> genreListCs = new List<string>();//
            foreach (var r in genreList)
            {
                if (r.Item2 > 0)
                {
                    genreListCs.Add(r.Item1 + ": " + r.Item2);
                }
                else
                {
                    genreListCs.Add(r.Item1);
                }
            }
            //for (int i = 0; i < genreListCs.Count; i++)
            //{
            //    checkedCheckboxesCs.Add("unchecked");
            //}
            return genreListCs;
        }

        public List<string> SetChecked(SongDto dto, List<string> genreListCs)
        {
            List<string> checkedCheckboxesCs = new List<string>();

            for (int i = 0; i < genreListCs.Count; i++)
            {
                checkedCheckboxesCs.Add("unchecked");
            }

            if (dto.CheckedCheckboxesJs != null)
            {
                foreach (int i in dto.CheckedCheckboxesJs)
                {
                    checkedCheckboxesCs[i - 1] = "checked";
                }
            }

            return checkedCheckboxesCs;
        }
    }
}
