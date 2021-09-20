﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Dto;
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
        private readonly ILogger<ReadController> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
 
        public ReadController(IServiceScopeFactory serviceScopeFactory, ILogger<ReadController> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            //_logger = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<ReadController>>();
        }

        [HttpGet]
        public async Task<ActionResult<SongDto>> OnGetGenreListAsync()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                return await new ReadModel(scope).ReadGenreListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReadController: OnGet Error]");
                return new SongDto() { ErrorMessageResponse = "[ReadController: OnGet Error]" };
            }
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> GetRandomSongAsync([FromBody] SongDto dto)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                return await new ReadModel(scope).ReadRandomSongAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReadController: OnPost Error]");
                return new SongDto() { ErrorMessageResponse = "[ReadController: OnPost Error]" };
            }
        }
    }
}