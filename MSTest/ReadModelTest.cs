using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using RandomSongSearchEngine.Repository;
using RandomSongSearchEngine.Models;
using Microsoft.Extensions.Logging;
using System;
using RandomSongSearchEngine.Data;
using Microsoft.EntityFrameworkCore;
using RandomSongSearchEngine.Dto;
using System.Collections.Generic;
using System.Linq;

namespace MSTest
{
    [TestClass]
    public class ReadModelTest
    {
        IServiceScope scope;
        ReadModel readModel;

        [TestInitialize]
        public void Initialize()
        {
            LogError.ExceptionMessage = "";
            LogError.LogErrorMessage = "";
            scope = new Scope().ServiceScope;
            readModel = new ReadModel(scope);
        }

        [TestMethod]
        public void AreThere44GenresTest()
        {
            var result = readModel.ReadGenreListAsync().Result;
            Assert.AreEqual(44, result.GenreListResponse.Count);
        }

        [TestMethod]
        public void AskForRandomSongTest()
        {
            var dto = new SongDto() { SongGenres = new List<int>() { 2 } };
            var result = readModel.ReadRandomSongAsync(dto).Result;
            Assert.AreEqual("2", result.TitleResponse);
        }

        [TestMethod]
        public void AskForWtfTest()
        {
            var dto = new SongDto() { SongGenres = new List<int>() { 1000 } };
            var result = readModel.ReadRandomSongAsync(dto).Result;
            Assert.AreEqual("", result.TitleResponse);
        }

        [TestMethod]
        public void IfNullTest1()
        {
            var result = readModel.ReadRandomSongAsync(null).Result;
            Assert.AreEqual("[IndexModel: OnPost Error]", LogError.LogErrorMessage);
        }

        [TestMethod]
        public void IfNullTest2()
        {
            var result = readModel.ReadRandomSongAsync(null).Result;
            Assert.AreEqual(null, result.TitleResponse);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            scope.Dispose();
        }
    }

    public class Scope
    {
        public readonly IServiceScope ServiceScope;
        private readonly string _connectionString = "Data Source=DESKTOP-I5CODE\\NEW3;Initial Catalog=rsse;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public Scope()
        {
            var services = new ServiceCollection();
            services.AddTransient<IRepository, MsSqlRepository>();
            services.AddTransient<ILogger<ReadModel>, MyLogger<ReadModel>>();
            services.AddDbContext<RsseContext>(options => options.UseSqlServer(_connectionString));
            var serviceProvider = services.BuildServiceProvider();
            ServiceScope = serviceProvider.CreateScope();
        }
    }

    public class MyLogger<ReadModel> : ILogger<ReadModel>
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
            LogError.ExceptionMessage = exception.Message;
            LogError.LogErrorMessage = state.ToString();
        }
    }

    public static class LogError
    {
        public static string ExceptionMessage { get; set; }
        public static string LogErrorMessage { get; set; }
    }
}