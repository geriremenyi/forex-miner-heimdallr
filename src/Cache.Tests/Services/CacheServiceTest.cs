namespace ForexMiner.Heimdallr.Cache.Tests.Services
{
    using ForexMiner.Heimdallr.Cache.Providers;
    using ForexMiner.Heimdallr.Cache.Services;
    using ForexMiner.Heimdallr.Cache.Utilities;
    using Moq;
    using System.Threading.Tasks;
    using Xunit;

    public class CacheServiceTest
    {
        private readonly Mock<IInMemoryCacheProvider> _inMemoryCacheProviderMock;
        private readonly Mock<IDistributedCacheProvider> _distributedCacheProviderMock;
        private readonly ICacheService _cacheService;

        public CacheServiceTest()
        {
            _inMemoryCacheProviderMock = new Mock<IInMemoryCacheProvider>();
            _distributedCacheProviderMock = new Mock<IDistributedCacheProvider>();
            _cacheService = new CacheService(_inMemoryCacheProviderMock.Object, _distributedCacheProviderMock.Object);
        }

        [Fact]
        public async void GetOrCreateCacheValue_CachePresentInLocalCache_DefaultLocalCreationTarget()
        {
            // Arrange
            var cacheType = CacheType.Secret;
            var cacheNamespace = "CacheTest";
            var cacheName = "NonExistingCacheKey";
            var cacheKey = $"{cacheType}-{cacheNamespace}-{cacheName}";

            string cacheLocalValue = "ThisIsFromTheLocalCache";
            string cacheDistributedValue = "ThisIsFromTheDistributedCache";
            var cacheProviderFunctionValue = "ThisIsFromTheCacheProviderFunction";

            string cacheValueProviderFunction() => cacheProviderFunctionValue;

            _inMemoryCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCacheValue(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction);

            // Assert
            _inMemoryCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(It.IsAny<string>()), Times.Never());
            _inMemoryCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            Assert.Equal(cacheLocalValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetOrCreateCacheValue_CachePresentInDistributedCache_DefaultLocalCreationTarget()
        {
            // Arrange
            var cacheType = CacheType.Secret;
            var cacheNamespace = "CacheTest";
            var cacheName = "NonExistingCacheKey";
            var cacheKey = $"{cacheType}-{cacheNamespace}-{cacheName}";

            string cacheLocalValue = null;
            string cacheDistributedValue = "ThisIsFromTheDistributedCache";
            var cacheProviderFunctionValue = "ThisIsFromTheCacheProviderFunction";

            string cacheValueProviderFunction() => cacheProviderFunctionValue;

            _inMemoryCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCacheValue(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction);

            // Assert
            _inMemoryCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey));
            _inMemoryCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, cacheDistributedValue));
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            Assert.Equal(cacheDistributedValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetOrCreateCacheValue_CacheNotPresent_DefaultLocalCreationTarget()
        {
            // Arrange
            var cacheType = CacheType.Secret;
            var cacheNamespace = "CacheTest";
            var cacheName = "NonExistingCacheKey";
            var cacheKey = $"{cacheType}-{cacheNamespace}-{cacheName}";

            string cacheLocalValue = null;
            string cacheDistributedValue = null;
            var cacheProviderFunctionValue = "ThisIsFromTheCacheProviderFunction";

            string cacheValueProviderFunction() => cacheProviderFunctionValue;

            _inMemoryCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCacheValue(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction);

            // Assert
            _inMemoryCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey));
            _inMemoryCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, cacheProviderFunctionValue));
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            Assert.Equal(cacheProviderFunctionValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetOrCreateCacheValue_CacheNotPresent_BothCreationTarget()
        {
            // Arrange
            var cacheType = CacheType.Secret;
            var cacheNamespace = "CacheTest";
            var cacheName = "NonExistingCacheKey";
            var cacheKey = $"{cacheType}-{cacheNamespace}-{cacheName}";

            string cacheLocalValue = null;
            string cacheDistributedValue = null;
            var cacheProviderFunctionValue = "ThisIsFromTheCacheProviderFunction";

            string cacheValueProviderFunction() => cacheProviderFunctionValue;

            _inMemoryCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCacheValue(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction, CacheCreateTarget.Both);

            // Assert
            _inMemoryCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey));
            _inMemoryCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, cacheProviderFunctionValue));
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, cacheProviderFunctionValue));
            Assert.Equal(cacheProviderFunctionValue, obtainedCacheValue);
        }
    }
}
