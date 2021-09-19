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
        IServiceScope scope;
        ReadModel readModel;

        [TestInitialize]
        public void Initialize()
        {
            LogError.ExceptionMessage = "";
            LogError.LogErrorMessage = "";
            scope = new MockScope().ServiceScope;
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
        public void IfNullLoggingErrorTest()
        {
            var result = readModel.ReadRandomSongAsync(null).Result;
            Assert.AreEqual("[IndexModel: OnPost Error]", LogError.LogErrorMessage);
        }

        [TestMethod]
        public void IfNullResponseNullInTitleTest()
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
}