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
    //[DisableCors]
    //[EnableCors]
    [ApiController]
    [Route("api/read")]

    public class ReadController : ControllerBase
    {
        private readonly ILogger<ReadModel> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
 
        public ReadController(IServiceScopeFactory serviceScopeFactory, ILogger<ReadModel> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> Index()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                return await new ReadModel(scope).OnGetReadAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReadController: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[ReadController: OnGet Error]" };
            }
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> Index([FromBody] SongDto dto)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                return await new ReadModel(scope).OnPostReadAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReadController: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[ReadController: OnPost Error]" }; ;
            }
        }
    }
}