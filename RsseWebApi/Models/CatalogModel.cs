﻿using Microsoft.EntityFrameworkCore;
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

        private const int Forward = 2;
        private const int Backward = 1;
        private const int MinimalPageNumber = 1;
        private static readonly int PageSize = 10;
        private readonly IServiceScope _scope;
        private readonly ILogger<CatalogModel> _logger;

        #endregion

        public CatalogModel(IServiceScope serviceScope)
        {
            _scope = serviceScope;
            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<CatalogModel>>();
        }

        public async Task<CatalogDto> OnGetAsync(int pageNumber)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>(); //
            try
            {
                int songsCount = await database.Text.CountAsync();
                List<Tuple<string, int>> catalogPage =
                    await database.ReadCatalogPageSql(pageNumber, PageSize).ToListAsync();
                return CreateCatalogDto(pageNumber, songsCount, catalogPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogModel]");
                return new CatalogDto() { ErrorMessage = "[CatalogModel]" };
            }
        }

        public async Task<CatalogDto> OnPostAsync(CatalogDto dto)
        {
            await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
            try
            {
                int navigation = dto.GetNavigation();
                int pageNumber = dto.PageNumber;
                int songsCount = await database.Text.CountAsync();
                pageNumber = Navigate(navigation, pageNumber, songsCount);
                List<Tuple<string, int>> catalogPage = await database
                    .ReadCatalogPageSql(pageNumber, PageSize).ToListAsync();
                return CreateCatalogDto(pageNumber, songsCount, catalogPage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogModel]");
                return new CatalogDto() { ErrorMessage = "[CatalogModel]" };
            }
        }

        private int Navigate(int navigation, int pageNumber, int songsCount)
        {
            if (navigation == Forward)
            {
                int pageCount = Math.DivRem(songsCount, PageSize, out int remainder);
                if (remainder > 0)
                {
                    pageCount++;
                }
                if (pageNumber < pageCount)
                {
                    pageNumber++;
                }
            }

            if (navigation == Backward)
            {
                if (pageNumber > MinimalPageNumber) pageNumber--;
            }

            return pageNumber;
        }

        private CatalogDto CreateCatalogDto(int pageNumber, int songsCount, List<Tuple<string, int>> catalogPage)
        {
            return new CatalogDto
            {
                PageNumber = pageNumber,
                CatalogPage = catalogPage ?? new List<Tuple<string, int>>(),
                SongsCount = songsCount
            };
        }
    }
}