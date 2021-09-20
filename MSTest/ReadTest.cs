using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RandomSongSearchEngine.Controllers;
using RandomSongSearchEngine.Dto;
using RandomSongSearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSTest
{
    [TestClass]
    public class ReadTest
    {
        const int GenresCount = 44;
        IServiceScope fakeScope;
        ReadModel readModel;

        [TestInitialize]
        public void Initialize()
        {
            FakeLoggerErrors.ExceptionMessage = "";
            FakeLoggerErrors.LogErrorMessage = "";
            fakeScope = new FakeScope<ReadModel>().ServiceScope;
            readModel = new ReadModel(fakeScope);
        }

        [TestMethod]
        public async Task ShouldBe44GenresTest()
        {
            var response = await readModel.ReadGenreListAsync();
            Assert.AreEqual(GenresCount, response.GenreListResponse.Count);
        }

        [TestMethod]
        public async Task ShouldReadRandomSongTest()
        {
            var request = new SongDto() { SongGenres = new List<int>() { 2 } };
            var response = await readModel.ReadRandomSongAsync(request);
            Assert.AreEqual("test title", response.TitleResponse);
        }

        [TestMethod]
        public async Task ShouldResponseEmpltyTitleIfWtfTest()
        {
            var frontRequest = new SongDto() { SongGenres = new List<int>() { 1000 } };
            var result = await readModel.ReadRandomSongAsync(frontRequest);
            Assert.AreEqual("", result.TitleResponse);
        }

        [TestMethod]
        public async Task IfNullShouldLoggingErrorInsideModelTest()
        {
            _ = await readModel.ReadRandomSongAsync(null);
            Assert.AreNotEqual("[IndexModel: OnPost Error]", FakeLoggerErrors.LogErrorMessage);
        }

        [TestMethod]
        public async Task IfNullShouldResponseEmptyTitleTest()
        {
            var response = await readModel.ReadRandomSongAsync(null);
            Assert.AreEqual("", response.TitleResponse);
        }

        [TestMethod]
        public async Task MockThrowsExceptionInsideControllerTest()
        {
            var mockLogger = Substitute.For<ILogger<ReadController>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.When(s => s.CreateScope()).Do(i => throw new Exception());
            var readController = new ReadController(fakeServiceScopeFactory, mockLogger);

            _ = await readController.GetRandomSongAsync(null);

            mockLogger.Received().LogError(Arg.Any<Exception>(), "[ReadController: OnPost Error]");
        }

        [TestMethod]
        public async Task MockShouldResponseEmptyTitleTest()
        {
            var mockLogger = Substitute.For<ILogger<ReadController>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.CreateScope().Returns(fakeScope);
            var readController = new ReadController(fakeServiceScopeFactory, mockLogger);

            var response = (await readController.GetRandomSongAsync(null)).Value;//.Result.Value;
            
            Assert.AreEqual("", response.TitleResponse);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            fakeScope.Dispose();
        }
    }
}