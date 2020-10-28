namespace ForexMiner.Heimdallr.Common.Caching.Tests.Providers
{
    using ForexMiner.Heimdallr.Common.Caching.Providers.InMemory;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    public class InMemoryCacheProviderTests
    {
        private readonly Mock<IMemoryCache> _innerCacheMock;
        private readonly IInMemoryCacheProvider _cacheProvider;

        public InMemoryCacheProviderTests()
        {
            _innerCacheMock = new Mock<IMemoryCache>();
            _cacheProvider = new InMemoryCacheProvider(_innerCacheMock.Object);
        }

        [Fact]
        public void Get()
        {
            // Arrange
            object cacheKey = "Unit-Test-Cache";
            object innerCacheValue = "MockCacheValue";
            _innerCacheMock.Setup(ic => ic.TryGetValue(cacheKey, out innerCacheValue));

            // Act
            var obtainedCacheValue = _cacheProvider.Get<string>(cacheKey.ToString());

            // Assert
            Assert.Equal(innerCacheValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetAsync()
        {
            // Arrange
            object cacheKey = "Unit-Test-Cache";
            object innerCacheValue = "MockCacheValue";
            _innerCacheMock.Setup(ic => ic.TryGetValue(cacheKey, out innerCacheValue));

            // Act
            var obtainedCacheValue = await _cacheProvider.GetAsync<string>(cacheKey.ToString());

            // Assert
            Assert.Equal(innerCacheValue, obtainedCacheValue);
        }

        [Fact]
        public void Set()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            string cacheValue = "MockCacheValue";
            _innerCacheMock.Setup(ic => ic.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            _cacheProvider.Set(cacheKey, cacheValue);

            // Assert
            _innerCacheMock.Verify(ic => ic.CreateEntry(cacheKey));
        }

        [Fact]
        public async void SetAsync()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            string cacheValue = "MockCacheValue";
            _innerCacheMock.Setup(ic => ic.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            await _cacheProvider.SetAsync(cacheKey, cacheValue);

            // Assert
            _innerCacheMock.Verify(ic => ic.CreateEntry(cacheKey));
        }

        [Fact]
        public void Remove()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            _innerCacheMock.Setup(ic => ic.Remove(cacheKey));

            // Act
            _cacheProvider.Remove(cacheKey);

            // Assert
            _innerCacheMock.Verify(ic => ic.Remove(cacheKey));
        }

        [Fact]
        public async void RemoveAsync()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            _innerCacheMock.Setup(ic => ic.Remove(cacheKey));

            // Act
            await _cacheProvider.RemoveAsync(cacheKey);

            // Assert
            _innerCacheMock.Verify(ic => ic.Remove(cacheKey));
        }
    }
}
