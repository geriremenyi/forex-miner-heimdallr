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
        private readonly Mock<ICacheProvider> _localCacheProviderMock;
        private readonly Mock<ICacheProvider> _distributedCacheProviderMock;
        private readonly ICacheService _cacheService;

        public CacheServiceTest()
        {
            _localCacheProviderMock = new Mock<ICacheProvider>();
            _distributedCacheProviderMock = new Mock<ICacheProvider>();
            _cacheService = new CacheService(_localCacheProviderMock.Object, _distributedCacheProviderMock.Object);
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

            _localCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCacheValue(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction);

            // Assert
            _localCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(It.IsAny<string>()), Times.Never());
            _localCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            Assert.Equal(cacheLocalValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetOrCreateCacheValue_CachePresentInLocalCache_BothCreationTarget()
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

            _localCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCacheValue(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction, CacheCreateTarget.Both);

            // Assert
            _localCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(It.IsAny<string>()), Times.Never());
            _localCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, cacheLocalValue));
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

            _localCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCacheValue(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction);

            // Assert
            _localCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey));
            _localCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, cacheDistributedValue));
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

            _localCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCacheValue(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction);

            // Assert
            _localCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey));
            _localCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, cacheProviderFunctionValue));
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

            _localCacheProviderMock.Setup(lcp => lcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheLocalValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(Task.FromResult(cacheDistributedValue));

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateCacheValue(cacheType, cacheNamespace, cacheName, cacheValueProviderFunction, CacheCreateTarget.Both);

            // Assert
            _localCacheProviderMock.Verify(lcp => lcp.Get<string>(cacheKey));
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey));
            _localCacheProviderMock.Verify(lcp => lcp.Set(cacheKey, cacheProviderFunctionValue));
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, cacheProviderFunctionValue));
            Assert.Equal(cacheProviderFunctionValue, obtainedCacheValue);
        }
    }
}
