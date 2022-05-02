using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomSongSearchEngine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using RandomSongSearchEngine.Data.DTO;

namespace MSTest;

[TestClass]
public class CreateTest
{
    private const int GenresCount = 44;
    private IServiceScope _fakeScope;
    private CreateModel _createModel;

    [TestInitialize]
    public void Initialize()
    {
        FakeLoggerErrors.ExceptionMessage = "";
        FakeLoggerErrors.LogErrorMessage = "";
        _fakeScope = new FakeScope<CreateModel>().ServiceScope;
        _createModel = new CreateModel(_fakeScope);
    }

    [TestMethod]
    public async Task ShouldBe44GenresTest()
    {
        var response = await _createModel.ReadGenreListAsync();
        Assert.AreEqual(GenresCount, response.GenreListResponse.Count);
    }

    [TestMethod]
    public async Task ShouldCreate()
    {
        var song = new SongDto()
        {
            Title = "test title",
            Text = "test text",
            SongGenres = new List<int>() {1, 2, 3, 4}
        };

        var response = await _createModel.CreateSongAsync(song);
        var expected =
            await new UpdateModel(new FakeScope<UpdateModel>().ServiceScope).ReadOriginalSongAsync(response.Id);
        Assert.AreEqual(expected.Title, response.Title);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _fakeScope.Dispose();
    }
}