using Microsoft.EntityFrameworkCore;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    public class BaseModel
    {
        /// <summary>
        /// Получаем список жанров с количеством песен в каждом
        /// </summary>
        /// <param name="database">Контекст бд для запроса</param>
        /// <returns>Список в виде строк с информацией</returns>
        public async Task<List<string>> GetGenreListAsync(RsseContext database)
        {
            List<string> genreListResponse = new List<string>();
            List<Tuple<string, int>> genreList = await database.ReadGenreListSql().ToListAsync();
            foreach (var genreAndAmount in genreList)
            {
                genreListResponse.Add(genreAndAmount.Item2 > 0 ? genreAndAmount.Item1 + ": " + genreAndAmount.Item2 : genreAndAmount.Item1);
            }
            return genreListResponse;
        }
    }
}