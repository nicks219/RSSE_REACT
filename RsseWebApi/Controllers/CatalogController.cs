using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Models;
using System;
using System.Threading.Tasks;

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
                using var scope = _serviceScopeFactory.CreateScope();
                return await new CatalogModel(scope).OnGetAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogController: OnGet Error]");
                return new CatalogDto() {ErrorMessage = "[CatalogController: OnGet Error]"};
            }
        }

        [HttpPost]
        public async Task<ActionResult<CatalogDto>> Catalog([FromBody] CatalogDto dto)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                return await new CatalogModel(scope).OnPostAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogController: OnPost Error]");
                return new CatalogDto() {ErrorMessage = "[CatalogController: OnGet Error]"};
            }
        }
    }
}