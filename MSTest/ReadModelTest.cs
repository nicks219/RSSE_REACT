using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomSongSearchEngine.Dto;
using RandomSongSearchEngine.Models;
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
            Assert.AreEqual("[IndexModel: OnPost Error]", FakeLoggerErrors.LogErrorMessage);
        }

        [TestMethod]
        public void IfNullResponseNullInTitleTest()
        {
            var response = readModel.ReadRandomSongAsync(null).Result;
            Assert.AreEqual(null, response.TitleResponse);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            fakeScope.Dispose();
        }
    }
}