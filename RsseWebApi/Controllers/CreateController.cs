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
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CreateController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> AddText()
        {
            try
            {
                var model = new SongModel(_serviceScopeFactory);
                await model.OnGetCreateAsync();
                return model.ModelToDto();
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
                var model = new SongModel(_serviceScopeFactory);
                model.DtoToModel(dto);
                await model.OnPostCreateAsync();
                if (model.SavedTextId == 0)
                {
                    //не критическая ошибка: например, песня с таким названием уже есть, или поля пустые
                    return model.ModelToDto();
                }
                return model.ModelToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateController: OnPost Error]");
                return new SongModel().ModelToDto();
            }
        }
    }
}