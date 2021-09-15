using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    /// <summary>
    /// Каталог песен
    /// </summary>
    public class CatalogModel
    {
        #region Fields

        private static readonly int PageSize = 10;
        private readonly IServiceScope _scope;
        private readonly ILogger<CatalogModel> _logger;

        #endregion

        public CatalogModel(IServiceScope serviceScopeFactory)
        {
            _scope = serviceScopeFactory;
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<CatalogModel>>();
        }

        /// <summary>
        /// Получаем листинг песен с их id по номеру страницы
        /// </summary>
        /// <param name="id">номер страницы</param>
        /// <returns></returns>
        public async Task<CatalogDto> OnGetCatalogAsync(int id)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>(); //
            try
            {
                int pageNumber = id;
                int songsCount = await database.Text.CountAsync();
                List<Tuple<string, int>> catalogPage =
                    await database.ReadCatalogPageSql(pageNumber, PageSize).ToListAsync();
                return CreateCatalogDto(pageNumber, songsCount, catalogPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogModel]");
                return new CatalogDto() {ErrorMessage = "[CatalogModel]"};
            }
        }

        /// <summary>
        /// Навигация по каталогу
        /// </summary>
        /// <param name="dto">запрос со стороны фронта</param>
        /// <returns></returns>
        public async Task<CatalogDto> OnPostCatalogAsync(CatalogDto dto)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                List<int> navigationButtons = dto.NavigationButtons;
                int pageNumber = dto.PageNumber;
                int songsCount = await database.Text.CountAsync();
                pageNumber = Navigate(navigationButtons, pageNumber, songsCount);
                List<Tuple<string, int>> catalogPage =
                    await database.ReadCatalogPageSql(pageNumber, PageSize).ToListAsync();
                return CreateCatalogDto(pageNumber, songsCount, catalogPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogModel]");
                return new CatalogDto() {ErrorMessage = "[CatalogModel]"};
            }
        }

        private int Navigate(List<int> navigationButtons, int pageNumber, int songsCount)
        {
            if (navigationButtons != null && navigationButtons[0] == 2)
            {
                int pageCount = Math.DivRem(songsCount, PageSize, out int remainder);
                if (remainder > 0) pageCount++;
                if (pageNumber < pageCount) pageNumber++;
            }

            if (navigationButtons != null && navigationButtons[0] == 1)
            {
                if (pageNumber > 1) pageNumber--;
            }

            return pageNumber;
        }

        private CatalogDto CreateCatalogDto(int pageNumber, int songsCount, List<Tuple<string, int>> catalogPage)
        {
            return new CatalogDto
            {
                PageNumber = pageNumber,
                TitlesAndIds = catalogPage ?? new List<Tuple<string, int>>(),
                SongsCount = songsCount
            };
        }
    }
}