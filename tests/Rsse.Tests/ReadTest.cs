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
using RandomSongSearchEngine.Tests.Mocks;

namespace RandomSongSearchEngine.Tests
{
    [TestClass]
    public class ReadTest
    {
        private const int GenresCount = 44;
        private IServiceScope? _fakeScope;
        private ReadModel? _readModel;

        [TestInitialize]
        public void Initialize()
        {
            FakeLoggerErrors.ExceptionMessage = "";
            FakeLoggerErrors.LogErrorMessage = "";
            _fakeScope = new FakeScope<ReadModel>().ServiceScope;
            _readModel = new ReadModel(_fakeScope);
        }

        [TestMethod]
        public async Task ShouldBe44GenresTest()
        {
            var response = await _readModel!.ReadGenreListAsync();
            Assert.AreEqual(GenresCount, response.GenreListResponse?.Count);
        }

        [TestMethod]
        public async Task ShouldReadRandomSongTest()
        {
            var request = new SongDto() { SongGenres = new List<int>() { 2 } };
            var response = await _readModel!.ReadRandomSongAsync(request);
            Assert.AreEqual("test title", response.TitleResponse);
        }

        [TestMethod]
        public async Task ShouldResponseEmpltyTitleIfWtfTest()
        {
            var frontRequest = new SongDto() { SongGenres = new List<int>() { 1000 } };
            var result = await _readModel!.ReadRandomSongAsync(frontRequest);
            Assert.AreEqual("", result.TitleResponse);
        }

        [TestMethod]
        public async Task IfNullShouldLoggingErrorInsideModelTest()
        {
            _ = await _readModel!.ReadRandomSongAsync(null!);
            Assert.AreNotEqual("[IndexModel: OnPost Error]", FakeLoggerErrors.LogErrorMessage);
        }

        [TestMethod]
        public async Task IfNullShouldResponseEmptyTitleTest()
        {
            var response = await _readModel!.ReadRandomSongAsync(null!);
            Assert.AreEqual("", response.TitleResponse);
        }

        [TestMethod]
        public async Task MockThrowsExceptionInsideControllerTest()
        {
            var mockLogger = Substitute.For<ILogger<ReadController>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.When(s => s.CreateScope()).Do(i => throw new Exception());
            var readController = new ReadController(fakeServiceScopeFactory, mockLogger);

            _ = await readController.GetRandomSongAsync(null!);

            mockLogger.Received().LogError(Arg.Any<Exception>(), "[ReadController: OnPost Error]");
        }

        [TestMethod]
        public async Task MockShouldResponseEmptyTitleTest()
        {
            var mockLogger = Substitute.For<ILogger<ReadController>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.CreateScope().Returns(_fakeScope);
            var readController = new ReadController(fakeServiceScopeFactory, mockLogger);

            var response = (await readController.GetRandomSongAsync(null!)).Value;//.Result.Value;
            
            Assert.AreEqual("", response?.TitleResponse);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _fakeScope?.Dispose();
        }
    }
}