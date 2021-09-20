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
    public class CreateTest
    {
        const int GenresCount = 44;
        IServiceScope fakeScope;
        CreateModel createModel;

        [TestInitialize]
        public void Initialize()
        {
            FakeLoggerErrors.ExceptionMessage = "";
            FakeLoggerErrors.LogErrorMessage = "";
            fakeScope = new FakeScope<CreateModel>().ServiceScope;
            createModel = new CreateModel(fakeScope);
        }

        [TestMethod]
        public async Task ShouldBe44GenresTest()
        {
            var response = await createModel.ReadGenreListAsync();
            Assert.AreEqual(GenresCount, response.GenreListResponse.Count);
        }

        [TestMethod]
        public async Task ShouldCreate()
        {
            var song = new SongDto()
            {
                Title = "test title",
                Text = "test text",
                SongGenres = new List<int>() { 1, 2, 3, 4 }
            };

            var response = await createModel.CreateSongAsync(song);
            var expected = await new UpdateModel(new FakeScope<UpdateModel>().ServiceScope).ReadOriginalSongAsync(response.Id);
            Assert.AreEqual(expected.Title, response.Title);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            fakeScope.Dispose();
        }
    }
}
