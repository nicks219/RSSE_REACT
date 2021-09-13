using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Models;
using System.Threading.Tasks;
using RandomSongSearchEngine.Extensions;
using System;

namespace RandomSongSearchEngine.Controllers
{
    //[Authorize]
    //[Route("api/[controller]")]
    [Route("api/catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogModel> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CatalogController(IServiceScopeFactory serviceScopeFactory, ILogger<CatalogModel> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<CatalogDto>> Catalog(int id)
        {
            try
            {
                var model = new CatalogModel(_serviceScopeFactory);
                await model.OnGetCatalogAsync(id);
                return model.CatalogToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogController: OnGet Error]");
                return new CatalogModel().CatalogToDto();
            }
        }

        [HttpPost]
        public async Task<ActionResult<CatalogDto>> Catalog([FromBody] CatalogDto dto)
        {
            try
            {
                var model = new CatalogModel(_serviceScopeFactory);
                model.DtoToCatalog(dto);
                await model.OnPostCatalogAsync();
                return model.CatalogToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogController: OnPost Error]");
                return new CatalogModel().CatalogToDto();
            }
        }
    }
}