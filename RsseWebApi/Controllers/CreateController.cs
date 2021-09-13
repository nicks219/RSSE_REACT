using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Models;
using System.Threading.Tasks;
using RandomSongSearchEngine.Extensions;

namespace RandomSongSearchEngine.Controllers
{
    [Authorize]
    [Route("api/create")]
    [ApiController]
    public class CreateController : ControllerBase
    {
        private readonly ILogger<SongModel> _logger;
        private readonly IServiceScopeFactory _scope;
        private readonly SongModel _model;

        public CreateController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            _logger = logger;
            _scope = serviceScopeFactory;
            _model = new SongModel(_scope, _logger);
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> AddText()
        {
            await _model.OnGetCreateAsync();
            return _model.ModelToDto();
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> AddText([FromBody] SongDto dto)
        {
            _model.ServiceScopeFactory = _scope;
            _model.Logger = _logger;
            _model.DtoToModel(dto);

            await _model.OnPostCreateAsync();
            if (_model.SavedTextId == 0)
            {
                //ошибка: например, песня с таким названием уже есть, или поля пустые
                return _model.ModelToDto();
            }
            return _model.ModelToDto();
        }
    }
}