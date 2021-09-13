using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using React.AspNet;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.ChakraCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using RandomSongSearchEngine.Services.Logger;

// TODO: ПЕРЕД ПУБЛИКАЦИЕЙ НЕ ЗАБУДЬ УБРАТЬ ПОДКЛЮЧЕНИЯ К БД
// TODO: сделай меню бутстраповское
//+ TODO: на данный момент 4 контроллера практически идентичны
//+ TODO: сделай шаг RESTfull, избавься от состояния (MySingleton) в бэке
//  TODO: сделай второй шаг - избавься от состояния во фронте)
// TODO: кнопку "все песни" попробуй как в site.js
// TODO: уменьши мастшаб экрана на сотовом в 2 раза при загрузке
//+ TODO: добавь логгер в контроллер
//+ V0.0.1 TODO: избавься от модели в контроллере (только dto)
// TODO: прикрути secret json))
// TODO: LoginController оформи по-человечески

namespace RandomSongSearchEngine
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Nick", Version = "v1" }); });
            services.AddDbContext<RsseContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMemoryCache();
            //для конфига REACT
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddReact();
            services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName).AddChakraCore();
            services.AddControllers();
            //services.AddControllersWithViews();
            //services.AddAuthentication(o => { }).AddCookie(options => { options.LoginPath = "/Account/Unauthorized/"; options.AccessDeniedPath = "/Account/Forbidden/"; });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login/");
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nick V1"); });
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            // переключение между indexMin, indexJsx (и razor pages если будут)
            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add(Configuration.GetValue<string>("StartFile"));
            app.UseDefaultFiles(options);
            app.UseReact(config => { });
            app.UseStaticFiles();
            //
            //app.UseHttpsRedirection();
            app.UseRouting();
            //
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
            var logger = loggerFactory.CreateLogger("FileLogger");
            logger.LogInformation("Application started at {0}, is 64-bit process: {1}", DateTime.Now, Environment.Is64BitProcess);
        }
    }
}

//ИСТОРИЯ ИЗМЕНЕНИЙ:
//V1.1.3 (коммит перед рефакторингом)
//V1.1.4 (убрал синглтон, 3 модели, лишние нэймспейсы и папки)
//V1.1.5 (частично сделаны css и верстка)
//V1.1.6 (деплой - доделан css для 1й страницы, jsx переведены в js)
// НАЛАДИЛ чтение jsx на хосте: nuGet chakra для x86 (+еще 2) - ушло два дня на эксперименты
// бандл static собран на ноде
// в cshml есть пример навороченного css
//V1.1.7 (решена проблема с хостингом, собран js в бандл)
//V1.1.9 слил connection string на git
//
//эти примеры кода пока не запомнил, пусть полежат
//
////код для CORS с форума
//services.AddCors(
//options =>
//{
//options.AddPolicy(
//"AllowCors",
//builder =>
//{
//    builder.AllowAnyOrigin().WithMethods(
//        HttpMethod.Get.Method,
//        HttpMethod.Put.Method,
//        HttpMethod.Post.Method,
//        HttpMethod.Delete.Method).AllowAnyHeader().WithExposedHeaders("CustomHeader");
//});
//});
//
//app.UseCors("AllowCors");

// для REACT и JSX
// не сработало
// See http://reactjs.net/ for more information. Example:
//config
//  .AddScript("~/js/First.jsx")
//  .AddScript("~/js/Second.jsx");
// Example:
//config
//  .SetLoadBabel(false)
//  .AddScriptWithoutTransform("~/js/bundle.server.js");
//
// мой КОРС:
//services.AddCors(
//options =>
//     {
//         options.AddPolicy(
//            "AllowCors",
//           builder =>
//           {
//               builder
//               //.SetIsOriginAllowedToAllowWildcardSubdomains("http://localhost:3000")
//               .AllowCredentials()
//               //.AllowAnyOrigin()
//               .WithOrigins("http://localhost:3000")
//               .WithMethods(
//               HttpMethod.Get.ToString(),//.Method,
//               HttpMethod.Put.ToString(),//.Method,
//               HttpMethod.Post.ToString(),//.Method,
//               HttpMethod.Delete.ToString())//.Method)
//               .AllowAnyHeader()
//               .WithExposedHeaders("CustomHeader");
//           });
//     });
//
//config.SetLoadBabel(false)
//config
//.AddScript("~/js/catalog.jsx")
//.AddScript("~/js/create.jsx")
//.AddScript("~/js/loader.jsx")
//.AddScript("~/js/login.jsx")
//.AddScript("~/js/menu.jsx")
//.AddScript("~/js/read.jsx")
//.AddScript("~/js/update.jsx")
//;
//
//!!! сейчас подключен AddControllersWithViews вместо AddControllers и UseDefaultFiles над UseReact !!!
// на данный момент 28.07.21 фронт на разоре, использует jsx файлы
// 1) !!! деплойный (и додеплойный) вариант с js также жив, достаточно включить UseDefaultFiles() !!!
// !!! для статических js просто правь index - загружай из папки jst
// 2) сейчас также подключен CORS для отдельной тестовой разработки фронта (+[enablecors] в ReadController)
// конфиг в реакте возможно лишнее
// тк научился пользоваться webpack - разор пэйж можно убирать, толку от reactjs в смысле сборки - ноль
// да, сборка фронта - create-react-app и npm build
// возможно, это реально интегрировать в sln
// в index.cshtml сейчас пробую бутстраповское меню
//
// скопировал нодовский билд в static + index (старый - indexDpl), убрал config из реакта, добавил UseDefaultFiles()
// в билде добавлен biitstrap (в index) - это 130+ кб
// т.е. между разработкой и продакшном (файлами index) мне "вручную" переключаться?
// отключил cors