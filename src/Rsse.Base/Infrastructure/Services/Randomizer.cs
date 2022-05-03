using Microsoft.EntityFrameworkCore;
using RandomSongSearchEngine.Data.Repository.Contracts;

namespace RandomSongSearchEngine.Infrastructure.Services;

public static class Randomizer
{
    private static readonly Random Random = new Random();

    // Возвращает Id случайно выбранной песни из заданных категорий
    public static async Task<int> ReadRandomIdAsync(this IRepository repo, List<int> songGenresRequest)
    {
        int[] checkedGenres = songGenresRequest.ToArray();
        int howManySongs = await repo.SelectAllSongsInGenres(checkedGenres).CountAsync();
        if (howManySongs == 0)
        {
            return 0;
        }

        int coin = GetRandom(howManySongs);
        var result = await repo.SelectAllSongsInGenres(checkedGenres)
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