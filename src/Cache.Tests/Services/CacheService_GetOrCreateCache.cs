namespace ForexMiner.Heimdallr.Cache.Tests.Services
{
    using ForexMiner.Heimdallr.Cache.Providers;
    using ForexMiner.Heimdallr.Cache.Services;
    using ForexMiner.Heimdallr.Cache.Utilities;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class CacheService_GetOrCreateCache
    {
        private readonly Mock<ICacheProvider> _localCacheProviderMock;
        private readonly Mock<ICacheProvider> _distributedCacheProviderMock;
        private readonly ICacheService _cacheService;

        public CacheService_GetOrCreateCache()
        {
            _localCacheProviderMock = new Mock<ICacheProvider>();
            _distributedCacheProviderMock = new Mock<ICacheProvider>();
            _cacheService = new CacheService(_localCacheProviderMock.Object, _distributedCacheProviderMock.Object);
        }

        [Fact]
        public async void GetOrCreateCache_CacheNotPresent()
        {
            // Arrange
            var cacheType = CacheType.Secret;
            var cacheNamespace = "CacheTest";
            var cacheName = "NonExistingCacheKey";
            var cacheKey = $"{cacheType}-{cacheNamespace}-{cacheName}";

            string cacheLocalValue = null;
            string cacheDistributedValue = null;
            var cacheProviderFunctionValue = "ThisIsFromTheCacheProviderFunction";

            Func<string> cacheValueProviderFunction = () => cacheProviderFunctionValue;

            _localCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCache(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction);

            // Assert
            _localCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey));
            _localCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, cacheProviderFunctionValue));
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, cacheProviderFunctionValue));
            Assert.Equal(cacheProviderFunctionValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetOrCreateCache_CachePresentInDistributedCache()
        {
            // Arrange
            var cacheType = CacheType.Secret;
            var cacheNamespace = "CacheTest";
            var cacheName = "NonExistingCacheKey";
            var cacheKey = $"{cacheType}-{cacheNamespace}-{cacheName}";

            string cacheLocalValue = null;
            string cacheDistributedValue = "ThisIsFromTheDistributedCache";
            var cacheProviderFunctionValue = "ThisIsFromTheCacheProviderFunction";

            Func<string> cacheValueProviderFunction = () => cacheProviderFunctionValue;

            _localCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCache(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction);

            // Assert
            _localCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey));
            _localCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, cacheProviderFunctionValue), Times.Never());
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, cacheProviderFunctionValue), Times.Never());
            Assert.Equal(cacheDistributedValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetOrCreateCache_CachePresentInLocalCache()
        {
            // Arrange
            var cacheType = CacheType.Secret;
            var cacheNamespace = "CacheTest";
            var cacheName = "NonExistingCacheKey";
            var cacheKey = $"{cacheType}-{cacheNamespace}-{cacheName}";

            string cacheLocalValue = "ThisIsFromTheLocalCache";
            string cacheDistributedValue = "ThisIsFromTheDistributedCache";
            var cacheProviderFunctionValue = "ThisIsFromTheCacheProviderFunction";

            Func<string> cacheValueProviderFunction = () => cacheProviderFunctionValue;

            _localCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCache(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction);

            // Assert
            _localCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey), Times.Never());
            _localCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, cacheProviderFunctionValue), Times.Never());
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, cacheProviderFunctionValue), Times.Never());
            Assert.Equal(cacheLocalValue, obtainedCacheValue);
        }
    }
}
