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
    public class UpdateTest
    {
        const int GenresCount = 44;
        IServiceScope fakeScope;
        UpdateModel updateModel;

        [TestInitialize]
        public void Initialize()
        {
            FakeLoggerErrors.ExceptionMessage = "";
            FakeLoggerErrors.LogErrorMessage = "";
            fakeScope = new FakeScope<UpdateModel>().ServiceScope;
            updateModel = new UpdateModel(fakeScope);
        }

        [TestMethod]
        public async Task ShouldBe44GenresTest()
        {
            var response = await updateModel.ReadOriginalSongAsync(1);
            Assert.AreEqual(GenresCount, response.GenreListResponse.Count);
        }

        [TestMethod]
        public async Task ShouldUpdateTest()
        {
            var song = new SongDto()
            {
                Title = "test title",
                Text = "test text text",
                SongGenres = new List<int>() { 1, 2, 3},
                Id = 1
            };
            //var expected = await updateModel.ReadOriginalSongAsync(1);
            var response = await updateModel.UpdateSongAsync(song);
            Assert.AreEqual("test text text", response.TextResponse);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            fakeScope.Dispose();
        }
    }
}
