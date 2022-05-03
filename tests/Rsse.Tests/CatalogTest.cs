using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RandomSongSearchEngine.Controllers;
using RandomSongSearchEngine.Data.DTO;
using RandomSongSearchEngine.Service.Models;
using RandomSongSearchEngine.Tests.Mocks;

namespace RandomSongSearchEngine.Tests;

[TestClass]
public class CatalogTest
{
    private const int PageSize = 10;
    private IServiceScope? _fakeScope;
    private CatalogModel? _catalogModel;

    [TestInitialize]
    public void Initialize()
    {
        FakeLoggerErrors.ExceptionMessage = "";
        FakeLoggerErrors.LogErrorMessage = "";
        _fakeScope = new FakeScope<CatalogModel>().ServiceScope;
        _catalogModel = new CatalogModel(_fakeScope);
    }

    [TestMethod]
    public async Task ShouldReadCatalogPageTest()
    {
        var response = await _catalogModel!.ReadCatalogPageAsync(1);
        // не знаю, что должен проверять этот тест, количество песен на странице ?
        Assert.AreEqual( /*PageSize*/response.SongsCount, response.CatalogPage?.Count);
    }

    [TestMethod]
    public async Task ShouldNavigateTest()
    {
        var request = new CatalogDto() {NavigationButtons = new List<int>() {1}, PageNumber = 1};
        var response = await _catalogModel!.NavigateCatalogAsync(request);
        // не знаю, что должен проверять этот тест, количество песен на странице ?
        Assert.AreEqual( /*PageSize*/response.SongsCount, response.CatalogPage?.Count);
    }

    [TestMethod]
    public async Task IfWtfShouldLoggingErrorInsideModelTest()
    {
        var frontRequest = new CatalogDto() {NavigationButtons = new List<int>() {1000, 2000}};
        var result = await _catalogModel!.NavigateCatalogAsync(frontRequest);
        //Assert.ThrowsException<Exception>(async () => await result);

        Assert.AreEqual("[CatalogModel: OnPost Error]", result.ErrorMessage);
    }

    [TestMethod]
    public async Task IfNullShouldLoggingErrorTest()
    {
        _ = await _catalogModel!.NavigateCatalogAsync(null!);
        Assert.AreEqual("[CatalogModel: OnPost Error]", FakeLoggerErrors.LogErrorMessage);
    }

    [TestMethod]
    public async Task IfWtfShouldResponseZeroSongsTest()
    {
        var response = await _catalogModel!.DeleteSongAsync(-300, -200);
        Assert.AreEqual(0, response.SongsCount);
    }

    [TestMethod]
    public async Task MockThrowsExceptionInsideControllerTest()
    {
        var mockLogger = Substitute.For<ILogger<CatalogController>>();
        var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
        fakeServiceScopeFactory.When(s => s.CreateScope()).Do(i => throw new Exception());
        var catalogController = new CatalogController(fakeServiceScopeFactory, mockLogger);

        _ = await catalogController.NavigateCatalogAsync(null!);

        mockLogger.Received().LogError(Arg.Any<Exception>(), "[CatalogController: OnPost Error]");
    }

    [TestMethod]
    public async Task MockShouldResponseEmptyTitleTest()
    {
        var mockLogger = Substitute.For<ILogger<CatalogController>>();
        var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
        fakeServiceScopeFactory.CreateScope().Returns(_fakeScope);
        var catalogController = new CatalogController(fakeServiceScopeFactory, mockLogger);

        var response = (await catalogController.OnDeleteSongAsync(-300, -200)).Value;

        Assert.AreEqual(null, response?.CatalogPage);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _fakeScope?.Dispose();
    }
}
