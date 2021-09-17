using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Dto;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    public class LoginModel
    {
        private readonly IServiceScope _scope;

        public LoginModel(IServiceScope scope)
        {
            _scope = scope;
        }

        public async Task<ClaimsIdentity> TryLogin(LoginDto dto)
        {
            try
            {
                if (dto.Email == null && dto.Password == null)
                {
                    return null;
                }

                await using var database = _scope.ServiceProvider.GetRequiredService<RsseContext>();
                UserEntity user = await database.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);
                if (user == null)
                {
                    return null;
                }

                var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Email) };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                //это отработает только в классе, унаследованном от ControllerBase
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                return id;
            }
            catch (Exception ex)
            {
                var _logger = _scope.ServiceProvider.GetRequiredService<ILogger<LoginModel>>();
                _logger.LogError(ex, "[LoginModel: System Error]");
                return null;
            }
        }
    }
}