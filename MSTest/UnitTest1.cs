using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using RandomSongSearchEngine.Extensions;
using RandomSongSearchEngine.Models;
using Microsoft.Extensions.Logging;
using System;
using RandomSongSearchEngine.Data;
using Microsoft.EntityFrameworkCore;

namespace MSTest
{
    [TestClass]
    public class UnitTest1
    {
        IServiceScope scope;
        ReadModel readModel;

        [TestInitialize]
        public void Initialize()
        {
            scope = new Scope().ServiceScope;
            readModel = new ReadModel(scope);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var result = readModel.ReadGenreListAsync().Result;
            Assert.AreEqual(44, result.GenreListResponse.Count);
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
            services.AddTransient<IDatabaseAccess, DatabaseAccess>();
            services.AddTransient<ILogger<ReadModel>, MyLogger<ReadModel>>();
            services.AddDbContext<RsseContext>(options => options.UseSqlServer(_connectionString));
            var serviceProvider = services.BuildServiceProvider();
            ServiceScope = serviceProvider.CreateScope();
        }
    }

    public class MyLogger<T> : ILogger<T>
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
            throw new NotImplementedException();
        }
    }
}
