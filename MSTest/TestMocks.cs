﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using System;
using RandomSongSearchEngine.Data.Repository;
using RandomSongSearchEngine.Data.Repository.Contracts;

namespace MSTest;

public class FakeScope<T> where T : class
{
    public readonly IServiceScope ServiceScope;

    // Connection String для MsSql
    // private readonly string _connectionString = "Data Source=DESKTOP-I5CODE\\SSDSQL;Initial Catalog=rsse;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

    // Connection String для MySql
    private const string ConnectionString = @"Server=localhost;Database=rsse;Uid=1;Pwd=1;";

    public FakeScope()
    {
        var services = new ServiceCollection();
        services.AddTransient<IRepository, MsSqlRepository>();
        services.AddTransient<ILogger<T>, FakeLogger<T>>();

        // MsSql
        // services.AddDbContext<RsseContext>(options => options.UseSqlServer(_connectionString));

        // MySql
        services.AddDbContext<RsseContext>(options =>
            options.UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 26))));

        // services.AddDbContext<RsseContext>(options => options.UseInMemoryDatabase(databaseName: "rsse"));
        var serviceProvider = services.BuildServiceProvider();
        ServiceScope = serviceProvider.CreateScope();
    }
}

public class FakeLogger<TReadModel> : ILogger<TReadModel>
{
    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        FakeLoggerErrors.ExceptionMessage = exception.Message;
        FakeLoggerErrors.LogErrorMessage = state.ToString();
    }
}

public static class FakeLoggerErrors
{
    public static string ExceptionMessage { get; set; }
    public static string LogErrorMessage { get; set; }
}