namespace ForexMiner.Heimdallr.Cache.Tests.Providers
{
    using ForexMiner.Heimdallr.Cache.Providers;
    using Microsoft.Extensions.Caching.Distributed;
    using Moq;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class DistributedCacheProviderTest
    {
        private readonly Mock<IDistributedCache> _innerCacheMock;
        private readonly ICacheProvider _cacheProvider;

        public DistributedCacheProviderTest()
        {
            _innerCacheMock = new Mock<IDistributedCache>();
            _cacheProvider = new DistributedCacheProvider(_innerCacheMock.Object);
        }

        [Fact]
        public async void Get()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            var innerCacheValue = JsonSerializer.SerializeToUtf8Bytes("MockCacheValue");

            _innerCacheMock.Setup(ic => ic.GetAsync(cacheKey, It.IsAny<CancellationToken>())).Returns(Task.FromResult(innerCacheValue));

            // Act
            var obtainedCacheValue = await _cacheProvider.Get<string>(cacheKey.ToString());

            // Assert
            _innerCacheMock.Verify(ic => ic.GetAsync(cacheKey, It.IsAny<CancellationToken>()));
            Assert.Equal(JsonSerializer.Deserialize<string>(innerCacheValue), obtainedCacheValue);
        }

        [Fact]
        public async void Set()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";
            var cacheValue = "MockCacheValue";
            var cacheValueSerialized = JsonSerializer.SerializeToUtf8Bytes(cacheValue);

            _innerCacheMock.Setup(ic => ic.SetAsync(cacheKey, cacheValueSerialized, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));

            // Act
            await _cacheProvider.Set(cacheKey, cacheValue);

            // Assert
            _innerCacheMock.Verify(ic => ic.SetAsync(cacheKey, cacheValueSerialized, It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void Remove()
        {
            // Arrange
            string cacheKey = "Unit-Test-Cache";

            _innerCacheMock.Setup(ic => ic.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()));

            // Act
            await _cacheProvider.Remove<string>(cacheKey);

            // Assert
            _innerCacheMock.Verify(ic => ic.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()));
        }
    }
}
