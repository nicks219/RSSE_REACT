using Microsoft.AspNetCore.Mvc;
using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Service.Models;

namespace RandomSongSearchEngine.Controllers;

// [Authorize]
[ApiController]
[Route("api/read")]

public class ReadController : ControllerBase
{
    private readonly ILogger<ReadController> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ReadController(IServiceScopeFactory serviceScopeFactory, ILogger<ReadController> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        // _logger = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<ReadController>>();
    }

    [HttpGet]
    public async Task<ActionResult<SongDto>> OnGetGenreListAsync()
    {
        // CORS ручная настройка
        // credentials: "include" или "same-origin"
        // HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
        // HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return await new ReadModel(scope).ReadGenreListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ReadController: OnGet Error]");
            return new SongDto() {ErrorMessageResponse = "[ReadController: OnGet Error]"};
        }
    }

    [HttpPost]
    public async Task<ActionResult<SongDto>> GetRandomSongAsync([FromBody] SongDto dto)
    {
        // CORS ручная настройка
        // HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
        // HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        // HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "POST");

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return await new ReadModel(scope).ReadRandomSongAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ReadController: OnPost Error]");
            return new SongDto() {ErrorMessageResponse = "[ReadController: OnPost Error]"};
        }
    }

    // CORS ручная настройка
    /*
    [HttpOptions]
    public ActionResult Options()
    {
        HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
        HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "OPTIONS");
        HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Content-type");
        return Ok();
    }
    */
}