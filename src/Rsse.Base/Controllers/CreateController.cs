﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Service.Models;

namespace RandomSongSearchEngine.Controllers;

[Authorize]
[Route("api/create")]
[ApiController]
public class CreateController : ControllerBase
{
    private readonly ILogger<CreateController> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CreateController(IServiceScopeFactory serviceScopeFactory, ILogger<CreateController> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<SongDto>> OnGetGenreListAsync()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var model = new CreateModel(scope);
            return await model.ReadGenreListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CreateController: OnGet Error]");
            return new SongDto() {ErrorMessageResponse = "[CreateController: OnGet Error]"};
        }
    }

    [HttpPost]
    public async Task<ActionResult<SongDto>> CreateSongAsync([FromBody] SongDto dto)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var model = new CreateModel(scope);
            return await model.CreateSongAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CreateController: OnPost Error]");
            return new SongDto() {ErrorMessageResponse = "[CreateController: OnPost Error]"};
        }
    }
}