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
        private readonly SongModel _model;

        public ReadController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            _logger = logger;
            _model = new SongModel(serviceScopeFactory);
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> Index()
        {
            try
            {
                await _model.OnGetReadAsync();
                return _model.ModelToDto();
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
                _model.DtoToModel(dto);
                await _model.OnPostReadAsync();
                return _model.ModelToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReadController: OnPost Error]");
                return new SongModel().ModelToDto();
            }
        }
    }
}