using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Services.Services;

namespace RandomSongSearchEngine.Extensions
{
    /// <summary>
    /// Экстеншены для работы с бд
    /// </summary>
    public static class SqlExtensions
    {
        /// <summary>
        /// Выборка названий жанров и количества песен в каждом из них
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <returns></returns>
        public static IQueryable<Tuple<string, int>> ReadGenreListSql(this RsseContext database)//
        {
            IQueryable<Tuple<string, int>> res = database.Genre
                .Select(g => new Tuple<string, int>(g.Genre, g.GenreTextInGenre.Count))
                .AsNoTracking();
            return res;
        }

        /// <summary>
        /// Выборка списка ID песен в отмеченных категориях
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="chosenOnes">Отмеченые категории</param>
        /// <returns></returns>
        public static IQueryable<int> ReadSongsIdForRandomSql(this RsseContext database, int[] chosenOnes)
        {
            //TODO определить какой лучше:
            //IQueryable<int> songsCollection = database.GenreText//
            //    .Where(s => chosenOnes.Contains(s.GenreInGenreText.GenreID))
            //    .Select(s => s.TextInGenreText.TextID);

            IQueryable<int> songsCollection = from a in database.Text
                                              where a.GenreTextInText.Any(c => chosenOnes.Contains(c.GenreId))
                                              select a.TextId;
            return songsCollection;
        }

        /// <summary>
        /// Выборка названий песен и их ID для CatalogModel
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="savedLastViewedPage">Текущая страница</param>
        /// <param name="pageSize">Количество песен на странице</param>
        /// <returns></returns>
        public static IQueryable<Tuple<string, int>> ReadCatalogPageSql(this RsseContext database, int savedLastViewedPage, int pageSize)
        {
            IQueryable<Tuple<string, int>> titleAndTextId = database.Text
                .OrderBy(s => s.Title)
                .Skip((savedLastViewedPage - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new Tuple<string, int>(s.Title, s.TextId))
                .AsNoTracking();

            return titleAndTextId;
        }

        /// <summary>
        /// Выборка текста и заголовка заданной песни
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="textId">ID песни</param>
        /// <returns></returns>
        public static IQueryable<Tuple<string, string>> ReadSongSql(this RsseContext database, int textId)
        {
            IQueryable<Tuple<string, string>> titleAndText = database.Text
                .Where(p => p.TextId == textId)
                .Select(s => new Tuple<string, string>(s.Song, s.Title))
                .AsNoTracking();
            return titleAndText;
        }

        /// <summary>
        /// Выборка категорий заданной песни для UpdateModel
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="savedTextId">ID песни</param>
        /// <returns></returns>
        public static IQueryable<int> ReadSongGenresSql(this RsseContext database, int savedTextId)
        {
            IQueryable<int> checkedList = database.GenreText
                .Where(p => p.TextInGenreText.TextId == savedTextId)
                .Select(s => s.GenreId);
            return checkedList;
        }

        /// <summary>
        /// Транзакция для изменению существующей песни
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        /// <param name="initialCheckboxes">Оригинальные категории песни</param>
        /// <param name="dt">Данные для песни</param>
        /// <returns></returns>
        public static async Task UpdateSongSqlAsync(this RsseContext db, List<int> initialCheckboxes, InnerDto dt)
        {
            // если имя заменено на существующее, то залоггируется исключение.
            // быстрой проверки нет - ресурсоёмко и ошибка редкая.
            // Id жанров из бд и номера кнопок с фронта совпадают
            await using IDbContextTransaction t = await db.Database.BeginTransactionAsync();
            try
            {
                TextEntity text = await db.Text.FindAsync(dt.SavedTextId);
                if (text == null)
                {
                    throw new Exception("[NULL in Text]");
                }
                HashSet<int> forAddition = dt.AreChecked.ToHashSet();
                HashSet<int> forDelete = initialCheckboxes.ToHashSet();
                List<int> except = forAddition.Intersect(forDelete).ToList();
                forAddition.ExceptWith(except);
                forDelete.ExceptWith(except);
                db.CheckGenresExistsError(dt.SavedTextId, forAddition);
                text.Title = dt.TitleFromHtml;
                text.Song = dt.TextFromHtml;
                db.Text.Update(text);
                db.GenreText.RemoveRange(db.GenreText.Where(f => f.TextId == dt.SavedTextId && forDelete.Contains(f.GenreId)));
                await db.GenreText.AddRangeAsync(forAddition.Select(genre => new GenreTextEntity { TextId = dt.SavedTextId, GenreId = genre }));
                await db.SaveChangesAsync();
                await t.CommitAsync();
            }
            //ошибка навигации либо введены существующие данные
            catch (DataExistsException)
            {
                await t.RollbackAsync();
            }
            catch (Exception)
            {
                await t.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Транзакция для добавлению новой песни
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        /// <param name="dt">Данные для песни</param>
        /// <returns>Возвращает ID добавленной песни или ноль при ошибке</returns>
        public static async Task<int> CreateSongSqlAsync(this RsseContext db, InnerDto dt)
        {
            await using IDbContextTransaction t = await db.Database.BeginTransactionAsync();
            try
            {
                db.CheckNameExistsError(dt.TitleFromHtml);
                TextEntity addition = new TextEntity { Title = dt.TitleFromHtml, Song = dt.TextFromHtml };
                await db.Text.AddAsync(addition);
                await db.SaveChangesAsync();
                await db.GenreText.AddRangeAsync(dt.AreChecked.Select(genre => new GenreTextEntity { TextId = addition.TextId, GenreId = genre }));
                await db.SaveChangesAsync();
                await t.CommitAsync();
                dt.SavedTextId = addition.TextId;
            }
            //ошибка навигации либо введены существующие данные
            catch (DataExistsException)
            {
                await t.RollbackAsync();
                dt.SavedTextId = 0;
            }
            //catch (DbUpdateException)
            catch (Exception)
            {
                await t.RollbackAsync();
                dt.SavedTextId = 0;
                throw;
            }

            return dt.SavedTextId;
        }

        /// <summary>
        /// Проверка консистентости данных по названию песни
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        /// <param name="title">Название песни</param>
        public static void CheckNameExistsError(this RsseContext db, string title)
        {
            int r = db.Text
                .Where(p => p.Title == title)
                .AsNoTracking()
                .Count();
            if (r > 0)
            {
                throw new DataExistsException("[Browser Refresh or Name Exists Error]");
            }
        }

        /// <summary>
        /// Проверка консистнентности данных по ID песни и категориям для добавления
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        /// <param name="savedTextId">ID песни</param>
        /// <param name="forAddition">Категории для добавления</param>
        public static void CheckGenresExistsError(this RsseContext db, int savedTextId, HashSet<int> forAddition)
        {
            if (forAddition.Count > 0)
            {
                int r = db.GenreText
                    .Where(p => p.TextId == savedTextId && p.GenreId == forAddition.First())
                    .AsNoTracking()
                    .Count();
                if (r > 0)
                {
                    throw new DataExistsException("[Browser Refresh or Genre Exists Error]");
                }
            }
        }
    }
}
