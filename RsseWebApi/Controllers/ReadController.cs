using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Models;
using System.Threading.Tasks;
using RandomSongSearchEngine.DTO;
using RandomSongSearchEngine.Extensions;

namespace RandomSongSearchEngine.Controllers
{
    //[Authorize]
    //[DisableCors]
    //[EnableCors]
    [ApiController]
    [Route("api/read")]

    public class ReadController : ControllerBase
    {
        public readonly ILogger<SongModel> Logger;
        private readonly IServiceScopeFactory _scope;
        private readonly SongModel _model;

        public ReadController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {

            Logger = logger;
            _scope = serviceScopeFactory;
            _model = new SongModel(_scope, Logger);
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> Index()
        {
            await _model.OnGetReadAsync();
            return _model.ModelToDto();
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> Index([FromBody] SongDto dto)
        {
            //model.AreChecked = _model.AreChecked;
            _model.ServiceScopeFactory = _scope;
            _model.Logger = Logger;
            _model.DtoToModel(dto);

            await _model.OnPostReadAsync();
            return _model.ModelToDto();
        }
    }
}