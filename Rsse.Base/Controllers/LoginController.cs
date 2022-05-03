﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using RandomSongSearchEngine.Data.DTO;

namespace RandomSongSearchEngine.Controllers;

[ApiController]
[Route("account")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly IServiceScopeFactory _scope;

    public LoginController(IServiceScopeFactory serviceScopeFactory, ILogger<LoginController> logger)
    {
        _logger = logger;
        _scope = serviceScopeFactory;
    }

    [HttpGet("login")]
    public async Task<ActionResult<string>> Login(string email, string password)
    {
        var loginModel = new LoginDto(email, password);
        var response = await Login(loginModel);
        return response == "[Ok]" ? "[LoginController: Login Ok]" : (ActionResult<string>) BadRequest(response);
    }

    [HttpGet("logout")]
    public async Task<ActionResult<string>> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return "[LoginController: Logout]";
    }

    private async Task<string> Login(LoginDto model)
    {
        using var scope = _scope.CreateScope();
        try
        {
            ClaimsIdentity? id = await new LoginModel(scope).TryLogin(model);
            if (id != null)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(id));
                return "[Ok]";
            }

            return "[LoginController: Data Error]";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LoginController: System Error]");
            return "[LoginController: System Error]";
        }
    }
}