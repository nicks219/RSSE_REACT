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
        private readonly ILogger<UpdateModel> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UpdateController(IServiceScopeFactory serviceScopeFactory, ILogger<UpdateModel> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> ChangeText(int id)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                return await new UpdateModel(scope).OnGetAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdateController: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[UpdateController: OnGet Error]" };
            }
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> ChangeText([FromBody] SongDto dto)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                return await new UpdateModel(scope).OnPostAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UpdateController: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[UpdateController: OnPost Error]" };
            }
        }
    }
}