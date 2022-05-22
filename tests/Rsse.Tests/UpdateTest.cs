using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Infrastructure.Cache;
using RandomSongSearchEngine.Service.Models;
using RandomSongSearchEngine.Tests.Infrastructure;

namespace RandomSongSearchEngine.Tests;

[TestClass]
public class UpdateTest
{
    private const int GenresCount = 44;
    
    private IServiceScope? _scope;
    
    private UpdateModel? _updateModel;

    private const string TestName = "test title";

    private const string TestText = "test text text";

    private int _testSongId;

    [TestInitialize]
    public void Initialize()
    {
        FakeLoggerErrors.ExceptionMessage = "";
        
        FakeLoggerErrors.LogErrorMessage = "";
        
        var scope = new TestScope<CacheRepository>().ServiceScope;
        
        var find = new FindModel(scope);

        _testSongId = find.FindIdByName(TestName);
        
        _scope = new TestScope<UpdateModel>().ServiceScope;
        
        _updateModel = new UpdateModel(_scope);
    }

    [TestMethod]
    public async Task Model_ShouldReports44Genres()
    {
        var response = await _updateModel!.ReadOriginalSongAsync(1);
        
        Assert.AreEqual(GenresCount, response.GenreListResponse?.Count);
    }

    [TestMethod]
    public async Task Model_ShouldUpdate()
    {
        var song = new SongDto
        {
            Title = TestName,
            Text = TestText,
            SongGenres = new List<int> {1, 2, 3, 11},
            Id = _testSongId
        };
        
        var response = await _updateModel!.UpdateSongAsync(song);
        
        Assert.AreEqual(TestText, response.TextResponse);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _scope?.Dispose();
    }
}
