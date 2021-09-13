using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Controllers
{
    [ApiController]
    [Route("account/{action}")]
    public class LoginController : ControllerBase
    {
        //private readonly RsseContext db;
        private readonly ILogger<SongModel> _logger;
        private readonly IServiceScopeFactory _scope;

        public LoginController(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            _logger = logger;
            _scope = serviceScopeFactory;
            //db = context;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Login(string returnurl, string email, string password)
        {
            var loginModel = new LoginModel
            {
                Email = email,
                Password = password
            };
            var a = await Login(loginModel);
            // либо данные
            var b = a.Value;
            // либо действие
            //var c = a.Result;
            // если кука прикрепилась - продолжается выполнение [Authorize] контроллера? разберись
            if (b == "ok") return "Authorize controller executed";
            return BadRequest(b);
        }


        /// <summary>
        /// Проверям логин и пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginModel model)
        {
            try
            {
                if (model.Email == null || model.Password == null) ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                if (ModelState.IsValid)
                {
                    using var scope = _scope.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<RsseContext>();
                    UserEntity user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                    if (user != null)
                    {
                        // аутентификация идёт успешно
                        await Authenticate(model.Email);
                        //return RedirectToAction("Index", "Read");
                        return "ok";
                    }
                    // нет пользователя
                    return "wrong_user";
                }
                // нет логина или пароля
                //return model;
                return "empty_data";
            }
            catch (Exception e)
            {
                // ошибка в бд
                _logger.LogError(e, "[LoginModel]");
                return "system_error";
            }
        }

        /// <summary>
        /// Прикрепляем куку к UserName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private async Task Authenticate(string userName)
        {
            // 1.создаем один claim
            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, userName) };

            // 2.создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            // 3.установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        /// <summary>
        /// Удаляем куки
        /// </summary>
        /// <param name="returnurl"></param>
        /// <returns></returns>
        public async Task<ActionResult<string>> Logout(string returnurl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return "Account controller executed: Logout";
            //return RedirectToAction("ChangeText", "update");
        }
    }
}
