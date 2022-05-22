using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RandomSongSearchEngine.Infrastructure.Cache;
using RandomSongSearchEngine.Tests.Infrastructure;

namespace RandomSongSearchEngine.Tests;

[TestClass]
public class CacheTests
{
    private readonly List<int> _testDef = new()
    {
        1040119440, 33759, 1030767639, 1063641, 2041410332, 1999758047, 1034259014,
        1796253404, 1201652179, 33583602, 1041276484, 1063641, 1911513819, -2036958882, 2001222215, 397889902,
        -242918757
    };
        
    private readonly List<int> _testUndef = new()
    {
        33551703, 33759, 1075359, 33449, 1034441666, 33361239, 1075421, 1034822160, 2003716344, 33790, 1087201,
        33449, 1080846, 33648454, 1993560527, 1035518482, 2031583174
    };
    
    private readonly IServiceScopeFactory _factory= Substitute.For<IServiceScopeFactory>();

    [TestInitialize]
    public void Initialize()
    {
        var scope = new TestScope<CacheRepository>(true).ServiceScope;
        _factory.CreateScope().Returns(scope);
    }

    [TestMethod]
    public void CacheRepository_ShouldInitCaches()
    {
        var cache = new CacheRepository(_factory);
        var def = cache.GetDefinedCache();
        var undef = cache.GetUndefinedCache();

        def.Should().NotBeNull();
        undef.Should().NotBeNull();
        def.Count.Should().Be(1);
        undef.Count.Should().Be(1);

        def.ElementAt(0)
            .Value
            .Should()
            .BeEquivalentTo(_testDef);
        
        undef.ElementAt(0)
            .Value
            .Should()
            .BeEquivalentTo(_testUndef);
    }
    
    [TestCleanup]
    public void TestCleanup()
    {
    }
}