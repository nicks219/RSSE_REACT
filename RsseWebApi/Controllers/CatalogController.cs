using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Models;
using System.Threading.Tasks;
using RandomSongSearchEngine.Extensions;

namespace RandomSongSearchEngine.Controllers
{
    //[Authorize]
    //[Route("api/[controller]")]
    [Route("api/catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<SongModel> _logger;
        private readonly IServiceScopeFactory _scope;
        private readonly CatalogModel _model;

        public CatalogController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            _logger = logger;
            _scope = serviceScopeFactory;
            _model = new CatalogModel(_scope, _logger);
        }

        [HttpGet]
        public async Task<ActionResult<CatalogDto>> Catalog(int id)
        {
            await _model.OnGetCatalogAsync(id);
            return _model.CatalogToDto();
        }

        [HttpPost]
        public async Task<ActionResult<CatalogModel>> Catalog([FromBody] CatalogDto dto)
        {
            _model.ServiceScopeFactory = _scope;
            _model.Logger = _logger;
            _model.DtoToCatalog(dto);

            await _model.OnPostCatalogAsync();
            return _model;
        }
    }
}