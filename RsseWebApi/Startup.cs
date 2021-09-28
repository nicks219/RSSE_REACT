using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Repository;
using RandomSongSearchEngine.Services.Logger;
using React.AspNet;
using System;
using System.Diagnostics;
using System.IO;
//MS Template .NET Core 3.1 SPA using: Microsoft.AspNetCore.SpaServices.Extensions 3.1.16

namespace RandomSongSearchEngine
{
    //- V0.0.9 для запросов Create, Read, Update(и Catalog) логика инкапсулирована в моделях, не имеющих зависимостей от ASP
    //  public методы моделей: OnGet(), OnPost() и (кроме Catalog) GetGenreListAsync(), public поля отсутствуют
    //  V1.0.0 выделен интерфейс IRepository - стало проще тестировать
    //  +TODO: рефакторинг LoginController и LoginModel
    //  +TODO: добавить запрос Delete
    //  +TODO: перевести фронт на TSX
    //  +TODO: сделай CRUD (post=create, get=read, put=update, delete=delete)
    //  ?TODO: разберись с неймингом в роутинге 
    //  TODO: полагаю, js билд (ClientApp/build) уместно создавать только в разработке, до publish
    //  TODO: разберись с опциями publish, не публикует папку ClientApp/build, public, src (только файлы *.json)
    //  +TODO: сделай переключение между MSSQL и MySQL

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
            // определим по connection string тип сервера и делегат DbContextOptionBuilder
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            string sqlServerType = "mysql";
            if (connectionString.Contains("Data Source")) 
            {
                sqlServerType = "mssql";
            }
            // ?: для делегата в C#8 не работает, потому свич
            Action<DbContextOptionsBuilder> dbOptions = sqlServerType switch
            {
                "mysql" => (options) => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26))),
                _ => (options) => options.UseSqlServer(connectionString),
            };
            services.AddDbContext<RsseContext>(dbOptions);

            services.AddMemoryCache();
            // для конфига REACT
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

            //SPA(1)
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            //
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            YarnRunBuild();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nick V1"); });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                //YarnRunBuild();
            }
            // переключение между indexMin, indexJsx (и razor pages если будут)
            //var options = new DefaultFilesOptions();
            //options.DefaultFileNames.Clear();
            //options.DefaultFileNames.Add(Configuration.GetValue<string>("StartFile"));
            //app.UseDefaultFiles(options);

            //app.UseReact(config => { });//
            // самый простой способ переключиться на TS
            //app.UseStaticFiles();
            //SPA(2)
            app.UseSpaStaticFiles();
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

            //SPA(3)
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
                else
                {
                    // скрипт из package.json (npm run build) построил билд, но не смог запустить kestrel, свалился
                    //if (Directory.Exists("")) ;
                    //spa.UseReactDevelopmentServer(npmScript: "build");
                }
            });
            //
            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
            var logger = loggerFactory.CreateLogger(typeof (FileLogger));
            logger.LogInformation("Application started at {0}, is 64-bit process: {1}", DateTime.Now, Environment.Is64BitProcess);
        }

        private void YarnRunBuild()
        {
            if (!Directory.Exists("./ClientApp/node_modules"))
            {
                // ноду надо ставить до старта - во время не получится, студия не скомпилирует typescript
                // перенесу в скрипт BuildEvents
                // yarn add --dev @types/react
                //string strCmdText = "/C cd ./ClientApp && npm install";
                //Process cmd = Process.Start("CMD.exe", strCmdText);
                //cmd.WaitForExit();
            }
            if (!Directory.Exists("./ClientApp/build"))
            {
                // yarn add --dev @types/react
                string strCmdText = "/C cd ./ClientApp && npm run build";
                //using {Process}
                Process cmd = Process.Start("CMD.exe", strCmdText);
                cmd.WaitForExit();
                //ProcessStartInfo cmdsi = new ProcessStartInfo("cmd.exe");
                //String command = @"/k java -jar myJava.jar";
                //cmdsi.Arguments = command;
                //Process cmd = Process.Start(cmdsi);
                //cmd.WaitForExit();
            }
        }
    }
}