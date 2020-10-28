namespace ForexMiner.Heimdallr.Common.Caching.Tests.Providers
{
    using ForexMiner.Heimdallr.Common.Caching.Providers.Distributed;
    using Microsoft.Extensions.Caching.Distributed;
    using Moq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class DistributedCacheProviderTests
    {
        private readonly Mock<IDistributedCache> _innerCacheMock;
        private readonly IDistributedCacheProvider _cacheProvider;

        public DistributedCacheProviderTests()
        {
            _innerCacheMock = new Mock<IDistributedCache>();
            _cacheProvider = new DistributedCacheProvider(_innerCacheMock.Object);
        }

        [Fact]
        public void Get()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            var innerCacheValue = JsonSerializer.SerializeToUtf8Bytes("MockCacheValue");
            _innerCacheMock.Setup(ic => ic.Get(cacheKey)).Returns(innerCacheValue);

            // Act
            var obtainedCacheValue = _cacheProvider.Get<string>(cacheKey.ToString());

            // Assert
            _innerCacheMock.Verify(ic => ic.Get(cacheKey));
            Assert.Equal(JsonSerializer.Deserialize<string>(innerCacheValue), obtainedCacheValue);
        }

        [Fact]
        public async void GetAsync()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            var innerCacheValue = JsonSerializer.SerializeToUtf8Bytes("MockCacheValue");

            _innerCacheMock.Setup(ic => ic.GetAsync(cacheKey, It.IsAny<CancellationToken>())).Returns(Task.FromResult(innerCacheValue));

            // Act
            var obtainedCacheValue = await _cacheProvider.GetAsync<string>(cacheKey.ToString());

            // Assert
            _innerCacheMock.Verify(ic => ic.GetAsync(cacheKey, It.IsAny<CancellationToken>()));
            Assert.Equal(JsonSerializer.Deserialize<string>(innerCacheValue), obtainedCacheValue);
        }

        [Fact]
        public void Set()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            var cacheValue = "MockCacheValue";
            var cacheValueSerialized = JsonSerializer.SerializeToUtf8Bytes(cacheValue);
            _innerCacheMock.Setup(ic => ic.Set(cacheKey, cacheValueSerialized, It.IsAny<DistributedCacheEntryOptions>()));

            // Act
            _cacheProvider.Set(cacheKey, cacheValue);

            // Assert
            _innerCacheMock.Verify(ic => ic.Set(cacheKey, cacheValueSerialized, It.IsAny<DistributedCacheEntryOptions>()));
        }

        [Fact]
        public async void SetAsync()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            var cacheValue = "MockCacheValue";
            var cacheValueSerialized = JsonSerializer.SerializeToUtf8Bytes(cacheValue);
            _innerCacheMock.Setup(ic => ic.SetAsync(cacheKey, cacheValueSerialized, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));

            // Act
            await _cacheProvider.SetAsync(cacheKey, cacheValue);

            // Assert
            _innerCacheMock.Verify(ic => ic.SetAsync(cacheKey, cacheValueSerialized, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
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

            _innerCacheMock.Setup(ic => ic.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()));

            // Act
            await _cacheProvider.RemoveAsync(cacheKey);

            // Assert
            _innerCacheMock.Verify(ic => ic.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()));
        }
    }
}
