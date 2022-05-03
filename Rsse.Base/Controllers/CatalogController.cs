using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Models;
using System;
using System.Threading.Tasks;
using RandomSongSearchEngine.Data.DTO;

namespace RandomSongSearchEngine.Controllers;

// [Authorize]
// [Route("api/[controller]")]
[Route("api/catalog")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly ILogger<CatalogController> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CatalogController(IServiceScopeFactory serviceScopeFactory, ILogger<CatalogController> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<CatalogDto>> OnGetCatalogPageAsync(int id)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return await new CatalogModel(scope).ReadCatalogPageAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CatalogController: OnGet Error]");
            return new CatalogDto() {ErrorMessage = "[CatalogController: OnGet Error]"};
        }
    }

    [HttpPost]
    public async Task<ActionResult<CatalogDto>> NavigateCatalogAsync([FromBody] CatalogDto dto)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return await new CatalogModel(scope).NavigateCatalogAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CatalogController: OnPost Error]");
            return new CatalogDto() {ErrorMessage = "[CatalogController: OnGet Error]"};
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<CatalogDto>> OnDeleteSongAsync(int id, int pg)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return await new CatalogModel(scope).DeleteSongAsync(id, pg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CatalogController: OnDelete Error]");
            return new CatalogDto() {ErrorMessage = "[CatalogController: OnDelete Error]"};
        }

    }
}
