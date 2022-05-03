using Microsoft.EntityFrameworkCore;
using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Data.Repository.Contracts;

namespace RandomSongSearchEngine.Service.Models;

public class CatalogModel
{
    #region Fields

    private const int Forward = 2;
    private const int Backward = 1;
    private const int MinimalPageNumber = 1;
    private const int PageSize = 10;
    private readonly IServiceScope _scope;
    private readonly ILogger<CatalogModel> _logger;

    #endregion

    public CatalogModel(IServiceScope serviceScope)
    {
        _scope = serviceScope;
        _logger = _scope.ServiceProvider.GetRequiredService<ILogger<CatalogModel>>();
    }

    public async Task<CatalogDto> ReadCatalogPageAsync(int pageNumber)
    {
        await using var repo = _scope.ServiceProvider.GetRequiredService<IRepository>(); //
        try
        {
            int songsCount = await repo.ReadTextsCountAsync();
            List<Tuple<string, int>> catalogPage =
                await repo.ReadCatalogPage(pageNumber, PageSize).ToListAsync();
            return CreateCatalogDto(pageNumber, songsCount, catalogPage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CatalogModel: OnGet Error]");
            return new CatalogDto() {ErrorMessage = "[CatalogModel: OnGet Error]"};
        }
    }

    public async Task<CatalogDto> NavigateCatalogAsync(CatalogDto catalog)
    {
        await using var repo = _scope.ServiceProvider.GetRequiredService<IRepository>();
        try
        {
            int direction = catalog.Direction();
            int pageNumber = catalog.PageNumber;
            int songsCount = await repo.ReadTextsCountAsync();
            pageNumber = NavigateCatalogPages(direction, pageNumber, songsCount);
            List<Tuple<string, int>> catalogPage = await repo.ReadCatalogPage(pageNumber, PageSize).ToListAsync();
            return CreateCatalogDto(pageNumber, songsCount, catalogPage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CatalogModel: OnPost Error]");
            return new CatalogDto() {ErrorMessage = "[CatalogModel: OnPost Error]"};
        }
    }

    public async Task<CatalogDto> DeleteSongAsync(int songId, int pageNumber)
    {
        await using var repo = _scope.ServiceProvider.GetRequiredService<IRepository>();
        try
        {
            await repo.DeleteSongAsync(songId);
            return await ReadCatalogPageAsync(pageNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CatalogModel: OnDelete Error]");
            return new CatalogDto() {ErrorMessage = "[CatalogModel: OnDelete Error]"};
        }
    }

    private static int NavigateCatalogPages(int navigation, int pageNumber, int songsCount)
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

    private static CatalogDto CreateCatalogDto(int pageNumber, int songsCount, List<Tuple<string, int>> catalogPage)
    {
        return new CatalogDto
        {
            PageNumber = pageNumber,
            CatalogPage = catalogPage ?? new List<Tuple<string, int>>(),
            SongsCount = songsCount
        };
    }
}