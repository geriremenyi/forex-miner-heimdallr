namespace ForexMiner.Heimdallr.Cache.Tests.Providers
{
    using ForexMiner.Heimdallr.Cache.Providers;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    public class InMemoryCacheProviderTest
    {
        private readonly Mock<IMemoryCache> _innerCacheMock;
        private readonly IInMemoryCacheProvider _cacheProvider;

        public InMemoryCacheProviderTest()
        {
            _innerCacheMock = new Mock<IMemoryCache>();
            _cacheProvider = new InMemoryCacheProvider(_innerCacheMock.Object);
        }

        [Fact]
        public async void Get()
        {
            // Arrange
            object cacheKey = "Unit-Test-Cache";
            object innerCacheValue = "MockCacheValue";

            _innerCacheMock.Setup(ic => ic.TryGetValue(cacheKey, out innerCacheValue));

            // Act
            var obtainedCacheValue = await _cacheProvider.Get<string>(cacheKey.ToString());

            // Assert
            Assert.Equal(innerCacheValue, obtainedCacheValue);
        }

        [Fact]
        public async void Set()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            string cacheValue = "MockCacheValue";

            _innerCacheMock.Setup(ic => ic.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            await _cacheProvider.Set(cacheKey, cacheValue);

            // Assert
            _innerCacheMock.Verify(ic => ic.CreateEntry(cacheKey));
        }

        [Fact]
        public async void Remove()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";

            _innerCacheMock.Setup(ic => ic.Remove(It.IsAny<object>()));

            // Act
            await _cacheProvider.Remove<string>(cacheKey);

            // Assert
            _innerCacheMock.Verify(ic => ic.Remove(cacheKey));
        }
    }
}
