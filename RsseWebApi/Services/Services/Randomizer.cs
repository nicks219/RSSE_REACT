using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Extensions;

namespace RandomSongSearchEngine.Services.Services
{
    /// <summary>
    /// Выборка случайной песни
    /// </summary>
    public static class Randomizer
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Возвращает ID случайно выбранной песни из выбраных категорий
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="songGenresRequest">Список выбраных категорий</param>
        /// <returns></returns>
        public static async Task<int> ReadRandomIdAsync(this IDatabaseAccess database, List<int> songGenresRequest)
        {
            int[] checkedGenres = songGenresRequest.ToArray();
            //IDatabaseAccess db = new DatabaseAccess(database);
            int howManySongs = await database.SelectSongIds(checkedGenres).CountAsync();
            if (howManySongs == 0)
            {
                return 0;
            }
            int coin = GetRandom(howManySongs);
            var result = await database.SelectSongIds(checkedGenres)
            //[WARNING] [Microsoft.EntityFrameworkCore.Query]  The query uses a row limiting operator ('Skip'/'Take')
            // without an 'OrderBy' operator.
                    .OrderBy(s => s)
                    .Skip(coin)
                    .Take(1)
                    .FirstAsync();
            return result;
        }

        /// <summary>
        /// Потокобезопасная генерация случайного числа в заданном диапазоне
        /// </summary>
        /// <param name="howMany">Количество песен, доступных для выборки</param>
        /// <returns></returns>
        private static int GetRandom(int howMany)
        {
            lock (Random)
            {
                int coin = Random.Next(0, howMany);
                return coin;
            }
        }
    }
}