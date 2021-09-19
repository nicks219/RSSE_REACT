﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.Models;
using RandomSongSearchEngine.Repository;
using System;

namespace MSTest
{
    public class FakeScope
    {
        public readonly IServiceScope ServiceScope;
        private readonly string _connectionString = "Data Source=DESKTOP-I5CODE\\NEW3;Initial Catalog=rsse;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public FakeScope()
        {
            var services = new ServiceCollection();
            services.AddTransient<IRepository, MsSqlRepository>();
            services.AddTransient<ILogger<ReadModel>, FakeLogger<ReadModel>>();
            services.AddDbContext<RsseContext>(options => options.UseSqlServer(_connectionString));
            var serviceProvider = services.BuildServiceProvider();
            ServiceScope = serviceProvider.CreateScope();
        }
    }

    public class FakeLogger<ReadModel> : ILogger<ReadModel>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
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
}