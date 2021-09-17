using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "[Empty email]")]
        public string Email { get; }

        [Required(ErrorMessage = "[Empty password]")]
        [DataType(DataType.Password)]
        public string Password { get; }

        public LoginModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public async Task<ClaimsIdentity> TryLogin(IServiceScope scope)
        {
            try
            {
                if (Email == null && Password == null)
                {
                    return null;
                }

                await using var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
                UserEntity user = await database.Users.FirstOrDefaultAsync(u => u.Email == Email && u.Password == Password);
                if (user == null)
                {
                    return null;
                }

                var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, Email) };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                //это отработает только в классе, унаследованном от ControllerBase
                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                return id;
            }
            catch (Exception ex)
            {
                var _logger = scope.ServiceProvider.GetRequiredService<ILogger<LoginModel>>();
                _logger.LogError(ex, "[LoginModel: System Error]");
                return null;
            }
        }
    }
}