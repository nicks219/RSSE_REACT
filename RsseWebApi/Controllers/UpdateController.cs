using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Models;
using System;
using System.Threading.Tasks;
using RandomSongSearchEngine.Data.DTO;

namespace RandomSongSearchEngine.Controllers;

[Authorize]
[Route("api/update")]
[ApiController]
public class UpdateController : ControllerBase
{
    private readonly ILogger<UpdateController> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UpdateController(IServiceScopeFactory serviceScopeFactory, ILogger<UpdateController> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<SongDto>> OnGetOriginalSongAsync(int id)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return await new UpdateModel(scope).ReadOriginalSongAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UpdateController: OnGet Error]");
            return new SongDto() {ErrorMessageResponse = "[UpdateController: OnGet Error]"};
        }
    }

    [HttpPost]
    public async Task<ActionResult<SongDto>> UpdateSongAsync([FromBody] SongDto dto)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return await new UpdateModel(scope).UpdateSongAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UpdateController: OnPost Error]");
            return new SongDto() {ErrorMessageResponse = "[UpdateController: OnPost Error]"};
        }
    }
}