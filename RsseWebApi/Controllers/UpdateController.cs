using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("api/update")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly ILogger<SongModel> _logger;
        private readonly SongModel _model;

        public UpdateController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            _logger = logger;
            _model = new SongModel(serviceScopeFactory);
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> ChangeText(int id)
        {
            try
            {
                //при id = 0 контроллер отдаст пустой список
                //if (id == 0) id = 1;
                await _model.OnGetUpdateAsync(id);
                return _model.ModelToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdateController: OnGet Error]");
                return new SongModel().ModelToDto();
            }
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> ChangeText([FromBody] SongDto dto)
        {
            try
            {
                //в полученной model будет пустое поле SongCount
                _model.DtoToModel(dto);
                await _model.OnPostUpdateAsync();
                return _model.ModelToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdateController: OnPost Error]");
                return new SongModel().ModelToDto();
            }
        }
    }
}