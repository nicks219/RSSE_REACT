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
using RandomSongSearchEngine.Repository;

namespace RandomSongSearchEngine
{
    //- V0.0.9 для запросов Create, Read, Update(и Catalog) логика инкапсулирована в моделях, не имеющих зависимостей от ASP
    //  public методы моделей: OnGet(), OnPost() и (кроме Catalog) GetGenreListAsync(), public поля отсутствуют
    //  V1.0.0 выделен интерфейс IRepository - стало проще тестировать
    //  +TODO: рефакторинг LoginController и LoginModel
    //  +TODO: добавить запрос Delete
    //  TODO: перевести фронт на TSX
    //  TODO: разберись с неймингом в роутинге и сделай CRUD (post=create, get=read, put=update, delete=delete)

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRepository, MsSqlRepository>();

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