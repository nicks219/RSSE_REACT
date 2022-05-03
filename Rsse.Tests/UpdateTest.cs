using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomSongSearchEngine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using RandomSongSearchEngine.Data.DTO;

namespace MSTest;

[TestClass]
public class UpdateTest
{
    private const int GenresCount = 44;
    private IServiceScope? _fakeScope;
    private UpdateModel? _updateModel;

    [TestInitialize]
    public void Initialize()
    {
        FakeLoggerErrors.ExceptionMessage = "";
        FakeLoggerErrors.LogErrorMessage = "";
        _fakeScope = new FakeScope<UpdateModel>().ServiceScope;
        _updateModel = new UpdateModel(_fakeScope);
    }

    [TestMethod]
    public async Task ShouldBe44GenresTest()
    {
        var response = await _updateModel!.ReadOriginalSongAsync(1);
        Assert.AreEqual(GenresCount, response.GenreListResponse?.Count);
    }

    [TestMethod]
    public async Task ShouldUpdateTest()
    {
        var song = new SongDto()
        {
            Title = "test title",
            Text = "test text text",
            SongGenres = new List<int>() {1, 2, 3},
            Id = 1
        };
        //var expected = await updateModel.ReadOriginalSongAsync(1);
        var response = await _updateModel!.UpdateSongAsync(song);
        Assert.AreEqual("test text text", response.TextResponse);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _fakeScope?.Dispose();
    }
}
