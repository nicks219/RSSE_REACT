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
    [Route("api/create")]
    [ApiController]
    public class CreateController : ControllerBase
    {
        private readonly ILogger<SongModel> _logger;
        private readonly SongModel _model;

        public CreateController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            _logger = logger;
            _model = new SongModel(serviceScopeFactory);
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> AddText()
        {
            try
            {
                await _model.OnGetCreateAsync();
                return _model.ModelToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateController: OnGet Error]");
                return new SongModel().ModelToDto();
            }
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> AddText([FromBody] SongDto dto)
        {
            try
            {
                _model.DtoToModel(dto);
                await _model.OnPostCreateAsync();
                if (_model.SavedTextId == 0)
                {
                    //не критическая ошибка: например, песня с таким названием уже есть, или поля пустые
                    return _model.ModelToDto();
                }
                return _model.ModelToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateController: OnPost Error]");
                return new SongModel().ModelToDto();
            }
        }
    }
}