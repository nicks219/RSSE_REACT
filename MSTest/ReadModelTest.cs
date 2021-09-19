using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RandomSongSearchEngine.Controllers;
using RandomSongSearchEngine.Dto;
using RandomSongSearchEngine.Models;
using System;
using System.Collections.Generic;

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
        public void AreThere44GenresTest()
        {
            var response = readModel.ReadGenreListAsync().Result;
            Assert.AreEqual(44, response.GenreListResponse.Count);
        }

        [TestMethod]
        public void AskForRandomSongTest()
        {
            var request = new SongDto() { SongGenres = new List<int>() { 2 } };
            var response = readModel.ReadRandomSongAsync(request).Result;
            Assert.AreEqual("2", response.TitleResponse);
        }

        [TestMethod]
        public void AskForWtfTest()
        {
            var frontRequest = new SongDto() { SongGenres = new List<int>() { 1000 } };
            var result = readModel.ReadRandomSongAsync(frontRequest).Result;
            Assert.AreEqual("", result.TitleResponse);
        }

        [TestMethod]
        public void IfNullLoggingErrorTest()
        {
            _ = readModel.ReadRandomSongAsync(null).Result;
            Assert.AreNotEqual("[IndexModel: OnPost Error]", FakeLoggerErrors.LogErrorMessage);
        }

        [TestMethod]
        public void IfNullResponseNullInTitleTest()
        {
            var response = readModel.ReadRandomSongAsync(null).Result;
            Assert.AreEqual("", response.TitleResponse);
        }

        [TestMethod]
        public void NSubstituteThrowExceptionTest()
        {
            var mockLogger = Substitute.For<ILogger<ReadModel>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.When(s => s.CreateScope()).Do(i => throw new Exception());
            var readController = new ReadController(fakeServiceScopeFactory, mockLogger);

            _ = readController.GetRandomSongAsync(null).Result;

            mockLogger.Received().LogError(Arg.Any<Exception>(), "[ReadController: OnPost Error]");
        }

        [TestMethod]
        public void NSubstituteTest()
        {
            var mockLogger = Substitute.For<ILogger<ReadModel>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.CreateScope().Returns(fakeScope);
            var readController = new ReadController(fakeServiceScopeFactory, mockLogger);

            var response = readController.GetRandomSongAsync(null).Result.Value;

            //??????
            Assert.ThrowsException<AggregateException>(() => readModel.ReadRandomSongAsync(null).Result);
            Assert.AreEqual("", response.TitleResponse);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            fakeScope.Dispose();
        }
    }
}