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
        private readonly ILogger<CreateModel> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CreateController(IServiceScopeFactory serviceScopeFactory, ILogger<CreateModel> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> AddText()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var model = new CreateModel(scope);
                return await model.OnGetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateController: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[CreateController: OnGet Error]" };
            }
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> AddText([FromBody] SongDto dto)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var model = new CreateModel(scope);
                return await model.OnPostAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateController: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[CreateController: OnPost Error]" };
            }
        }
    }
}