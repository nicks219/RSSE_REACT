using System;
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
        private readonly ILogger<SongModel> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
 
        public ReadController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> Index()
        {
            try
            {
                var model = new SongModel(_serviceScopeFactory);
                await model.OnGetReadAsync();
                return model.ModelToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReadController: OnGet Error]");
                return new SongModel().ModelToDto();
            }
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> Index([FromBody] SongDto dto)
        {
            try
            {
                var model = new SongModel(_serviceScopeFactory);
                model.DtoToModel(dto);
                await model.OnPostReadAsync();
                return model.ModelToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReadController: OnPost Error]");
                return new SongModel().ModelToDto();
            }
        }
    }
}