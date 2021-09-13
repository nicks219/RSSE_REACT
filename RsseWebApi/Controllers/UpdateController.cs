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
    [Route("api/update")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly ILogger<SongModel> _logger;
        private readonly IServiceScopeFactory _scope;
        private readonly SongModel _model;

        public UpdateController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            _logger = logger;
            _scope = serviceScopeFactory;
            _model = new SongModel(_scope, _logger);
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> ChangeText(int id)
        {
            //при id = 0 контроллер отдаст пустой список
            //if (id == 0) id = 1;
            await _model.OnGetUpdateAsync(id);
            return _model.ModelToDto();
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> ChangeText([FromBody] SongDto dto)
        {
            //в полученной model будет пустое поле SongCount
            _model.ServiceScopeFactory = _scope;
            _model.Logger = _logger;
            _model.DtoToModel(dto);

            await _model.OnPostUpdateAsync();
            return _model.ModelToDto();
        }
    }
}