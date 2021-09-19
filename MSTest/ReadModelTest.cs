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
    public class ReadModelTest
    {
        IServiceScope fakeScope;
        ReadModel readModel;

        [TestInitialize]
        public void Initialize()
        {
            FakeLoggerErrors.ExceptionMessage = "";
            FakeLoggerErrors.LogErrorMessage = "";
            fakeScope = new FakeScope().ServiceScope;
            readModel = new ReadModel(fakeScope);
        }

        [TestMethod]
        public async Task AreThere44GenresTest()
        {
            var response = await readModel.ReadGenreListAsync();
            Assert.AreEqual(44, response.GenreListResponse.Count);
        }

        [TestMethod]
        public async Task AskForRandomSongTest()
        {
            var request = new SongDto() { SongGenres = new List<int>() { 2 } };
            var response = await readModel.ReadRandomSongAsync(request);
            Assert.AreEqual("2", response.TitleResponse);
        }

        [TestMethod]
        public async Task AskForWtfTest()
        {
            var frontRequest = new SongDto() { SongGenres = new List<int>() { 1000 } };
            var result = await readModel.ReadRandomSongAsync(frontRequest);
            Assert.AreEqual("", result.TitleResponse);
        }

        [TestMethod]
        public async Task IfNullLoggingErrorTest()
        {
            _ = await readModel.ReadRandomSongAsync(null);
            Assert.AreNotEqual("[IndexModel: OnPost Error]", FakeLoggerErrors.LogErrorMessage);
        }

        [TestMethod]
        public async Task IfNullResponseNullInTitleTest()
        {
            var response = await readModel.ReadRandomSongAsync(null);
            Assert.AreEqual("", response.TitleResponse);
        }

        [TestMethod]
        public async Task NSubstituteThrowExceptionTest()
        {
            var mockLogger = Substitute.For<ILogger<ReadModel>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.When(s => s.CreateScope()).Do(i => throw new Exception());
            var readController = new ReadController(fakeServiceScopeFactory, mockLogger);

            _ = await readController.GetRandomSongAsync(null);

            mockLogger.Received().LogError(Arg.Any<Exception>(), "[ReadController: OnPost Error]");
        }

        [TestMethod]
        public async Task NSubstituteTest()
        {
            var mockLogger = Substitute.For<ILogger<ReadModel>>();
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