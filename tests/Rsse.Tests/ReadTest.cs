using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RandomSongSearchEngine.Controllers;
using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Service.Models;
using RandomSongSearchEngine.Tests.Infrastructure;

namespace RandomSongSearchEngine.Tests
{
    [TestClass]
    public class ReadTest
    {
        private const int GenresCount = 44;
        
        private IServiceScope? _scope;
        
        private ReadModel? _readModel;

        [TestInitialize]
        public void Initialize()
        {
            FakeLoggerErrors.ExceptionMessage = "";
            
            FakeLoggerErrors.LogErrorMessage = "";
            
            _scope = new TestScope<ReadModel>().ServiceScope;
            
            _readModel = new ReadModel(_scope);
        }

        [TestMethod]
        public async Task Model_ShouldReports44Genres()
        {
            var response = await _readModel!.ReadGenreListAsync();
            
            Assert.AreEqual(GenresCount, response.GenreListResponse?.Count);
        }

        [TestMethod]
        public async Task Model_ShouldReadRandomSong()
        {
            // интеграционные тесты следует проводить на тестовой бд в docker'е
            var request = new SongDto { SongGenres = new List<int> { 11 } };
            
            var response = await _readModel!.ReadRandomSongAsync(request);
            
            Assert.AreEqual("test title", response.TitleResponse);
        }

        [TestMethod]
        public async Task ModelInvalidRequest_ShouldResponseEmptyTitle()
        {
            var frontRequest = new SongDto { SongGenres = new List<int> { 1000 } };
            
            var result = await _readModel!.ReadRandomSongAsync(frontRequest);
            
            Assert.AreEqual("", result.TitleResponse);
        }

        [TestMethod]
        public async Task ModelNullRequest_ShouldLoggingErrorInsideModel()
        {
            _ = await _readModel!.ReadRandomSongAsync(null!);
            
            Assert.AreNotEqual("[IndexModel: OnPost Error]", FakeLoggerErrors.LogErrorMessage);
        }

        [TestMethod]
        public async Task ModelNullRequest_ShouldResponseEmptyTitleTest()
        {
            var response = await _readModel!.ReadRandomSongAsync(null!);
            
            Assert.AreEqual("", response.TitleResponse);
        }

        [TestMethod]
        public async Task ControllerThrowsException_ShouldLogError()
        {
            var mockLogger = Substitute.For<ILogger<ReadController>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.When(s => s.CreateScope()).Do(i => throw new Exception());
            var readController = new ReadController(fakeServiceScopeFactory, mockLogger);

            _ = await readController.GetRandomSongAsync(null!);

            mockLogger.Received().LogError(Arg.Any<Exception>(), "[ReadController: OnPost Error]");
        }

        [TestMethod]
        public async Task ControllerNullRequest_ShouldResponseEmptyTitle()
        {
            var mockLogger = Substitute.For<ILogger<ReadController>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.CreateScope().Returns(_scope);
            var readController = new ReadController(fakeServiceScopeFactory, mockLogger);

            var response = (await readController.GetRandomSongAsync(null!)).Value;
            
            Assert.AreEqual("", response?.TitleResponse);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _scope?.Dispose();
        }
    }
}