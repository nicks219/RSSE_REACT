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
    public class CatalogTest
    {
        const int PageSize = 10;
        IServiceScope fakeScope;
        CatalogModel catalogModel;

        [TestInitialize]
        public void Initialize()
        {
            FakeLoggerErrors.ExceptionMessage = "";
            FakeLoggerErrors.LogErrorMessage = "";
            fakeScope = new FakeScope<CatalogModel>().ServiceScope;
            catalogModel = new CatalogModel(fakeScope);
        }

        [TestMethod]
        public async Task ShouldReadCatalogPageTest()
        {
            var response = await catalogModel.ReadCatalogPageAsync(1);
            Assert.AreEqual(/*PageSize*/1, response.CatalogPage.Count);
        }

        [TestMethod]
        public async Task ShouldNavigateTest()
        {
            var request = new CatalogDto() { NavigationButtons = new List<int>() { 1 }, PageNumber = 1 };
            var response = await catalogModel.NavigateCatalogAsync(request);
            Assert.AreEqual(/*PageSize*/1, response.CatalogPage.Count);
        }

        [TestMethod]
        public async Task IfWtfShouldLoggingErrorInsideModelTest()
        {
            var frontRequest = new CatalogDto() { NavigationButtons = new List<int>() { 1000, 2000 } };
            var result = await catalogModel.NavigateCatalogAsync(frontRequest);
            //Assert.ThrowsException<Exception>(async () => await result);

            Assert.AreEqual("[CatalogModel: OnPost Error]", result.ErrorMessage);
        }

        [TestMethod]
        public async Task IfNullShouldLoggingErrorTest()
        {
            _ = await catalogModel.NavigateCatalogAsync(null);
            Assert.AreEqual("[CatalogModel: OnPost Error]", FakeLoggerErrors.LogErrorMessage);
        }

        [TestMethod]
        public async Task IfWtfShouldResponseZeroSongsTest()
        {
            var response = await catalogModel.DeleteSongAsync(-300, -200);
            Assert.AreEqual(0, response.SongsCount);
        }

        [TestMethod]
        public async Task MockThrowsExceptionInsideControllerTest()
        {
            var mockLogger = Substitute.For<ILogger<CatalogController>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.When(s => s.CreateScope()).Do(i => throw new Exception());
            var catalogController = new CatalogController(fakeServiceScopeFactory, mockLogger);

            _ = await catalogController.NavigateCatalogAsync(null);

            mockLogger.Received().LogError(Arg.Any<Exception>(), "[CatalogController: OnPost Error]");
        }

        [TestMethod]
        public async Task MockShouldResponseEmptyTitleTest()
        {
            var mockLogger = Substitute.For<ILogger<CatalogController>>();
            var fakeServiceScopeFactory = Substitute.For<IServiceScopeFactory>();
            fakeServiceScopeFactory.CreateScope().Returns(fakeScope);
            var catalogController = new CatalogController(fakeServiceScopeFactory, mockLogger);

            var response = (await catalogController.OnDeleteSongAsync(-300, -200)).Value;

            Assert.AreEqual(null, response.CatalogPage);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            fakeScope.Dispose();
        }
    }
}
