using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Services.Logger;
using System;
using System.IO;
using RandomSongSearchEngine.Data.Repository;
using RandomSongSearchEngine.Data.Repository.Contracts;

namespace RandomSongSearchEngine;

// в хосте настроены политики CORS
// фронт после билда необходимо скопировать из FrontRepository/ClientApp/build в RandomSongSearchEngine/ClientApp/build
// в проекте есть docker-compose и несколько dockerfile
// в appsettings.Production.json в качестве сервера бд указано имя сервиса из docker-compose (mysql) - требуется только для работы в контейнере

public class Startup
{
    private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();

        services.AddScoped<IRepository, MsSqlRepository>();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo {Title = "Nick", Version = "v1"});
        });

        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        var sqlServerType = connectionString.Contains("Data Source") ? "mssql" : "mysql";

        Action<DbContextOptionsBuilder> dbOptions = sqlServerType switch
        {
            "mysql" => (options) => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26))),
            _ => (options) => options.UseSqlServer(connectionString),
        };

        services.AddDbContext<RsseContext>(dbOptions);

        services.AddMemoryCache(); // ???

        services.AddControllers();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => { options.LoginPath = new PathString("/Account/Login/"); });
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

        // настраиваем на index.html
        app.UseDefaultFiles();
        app.UseStaticFiles();

        // app.UseHttpsRedirection();
        app.UseRouting();

        // CORS (нет в репе)
        // credentials: "include"
        app.UseCors(builder =>
        {
            builder.WithOrigins("http://localhost:3000").AllowCredentials();
            builder.WithHeaders("Content-type");
            builder.WithMethods("GET", "POST", "OPTIONS");
        });
        //

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
        var logger = loggerFactory.CreateLogger(typeof(FileLogger));
        logger.LogInformation("Application started at {0}, is 64-bit process: {1}", DateTime.Now,
            Environment.Is64BitProcess);
        logger.LogInformation($"IsDevelopment: {env.IsDevelopment()}");
        logger.LogInformation($"IsProduction: {env.IsProduction()}");
        logger.LogInformation($"Connection string here: {_configuration.GetConnectionString("DefaultConnection")}");
    }
}