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
        private readonly CatalogModel _model;

        public CatalogController(IServiceScopeFactory serviceScopeFactory, ILogger<CatalogModel> logger)
        {
            _logger = logger;
            _model = new CatalogModel(serviceScopeFactory);
        }

        [HttpGet]
        public async Task<ActionResult<CatalogDto>> Catalog(int id)
        {
            try
            {
                await _model.OnGetCatalogAsync(id);
                return _model.CatalogToDto();
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
                _model.DtoToCatalog(dto);
                await _model.OnPostCatalogAsync();
                return _model.CatalogToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogController: OnPost Error]");
                return new CatalogModel().CatalogToDto();
            }
        }
    }
}