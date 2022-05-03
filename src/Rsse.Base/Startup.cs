﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Data.Repository;
using RandomSongSearchEngine.Data.Repository.Contracts;
using RandomSongSearchEngine.Infrastructure.Logger;

namespace RandomSongSearchEngine;

// в хосте настроены политики CORS
// фронт после билда необходимо скопировать из FrontRepository/ClientApp/build в Rsse.Base/ClientApp/build
// в проекте есть docker-compose и несколько dockerfile
// appsettings.Production.json - для запуска в контейнере, сервер бд - имя сервиса из docker-compose (mysql)
// appsettings.Development.json - для запуска вне контейнера, сервер бд - localhost или 127.0.0.1

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

        services.AddScoped<IRepository, RsseRepository>();

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
            builder.WithOrigins("http://localhost:3000", "http://localhost:5000").AllowCredentials();
            builder.WithHeaders("Content-type");
            builder.WithMethods("GET", "POST", "OPTIONS");
        });
        //

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
        var logger = loggerFactory.CreateLogger(typeof(FileLogger));
        logger.LogInformation("Application started at {Date}, is 64-bit process: {Process}", 
            DateTime.Now.ToString(CultureInfo.InvariantCulture), Environment.Is64BitProcess.ToString());
        logger.LogInformation("IsDevelopment: {IsDev}", env.IsDevelopment().ToString());
        logger.LogInformation("IsProduction: {IsProd}", env.IsProduction().ToString());
        logger.LogInformation("Connection string here: {ConnectionString}", 
            _configuration.GetConnectionString("DefaultConnection"));
    }
}