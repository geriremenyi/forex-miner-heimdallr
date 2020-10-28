namespace ForexMiner.Heimdallr.Common.Caching.Tests.Services
{
    using ForexMiner.Heimdallr.Common.Caching.Providers.Distributed;
    using ForexMiner.Heimdallr.Common.Caching.Providers.InMemory;
    using ForexMiner.Heimdallr.Common.Caching.Services;
    using Moq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class CachingServiceTests
    {
        private readonly Mock<IInMemoryCacheProvider> _inMemoryCacheProviderMock;
        private readonly Mock<IDistributedCacheProvider> _distributedCacheProviderMock;
        private readonly ICachingService _cacheService;

        public CachingServiceTests()
        {
            _inMemoryCacheProviderMock = new Mock<IInMemoryCacheProvider>();
            _distributedCacheProviderMock = new Mock<IDistributedCacheProvider>();
            _cacheService = new CachingService(_inMemoryCacheProviderMock.Object, _distributedCacheProviderMock.Object);
        }

        [Fact]
        public void GetOrCreateValue_FromInMemoryProvider()
        {
            // Arrange
            var cacheKey = "Unit-Test-Cache";
            var inMemoryValue = "FromInMemory";
            var distributedValue = "FromDistributed";
            var providerFunctionValue = "FromProviderFunction";

            _inMemoryCacheProviderMock.Setup(imcp => imcp.Get<string>(cacheKey)).Returns(inMemoryValue);
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(distributedValue);
            string cacheProviderFunc() => providerFunctionValue;

            // Act
            var obtainedCacheValue = _cacheService.GetOrCreateValue(cacheKey, cacheProviderFunc);

            // Assert
            _inMemoryCacheProviderMock.Verify(imcp => imcp.Get<string>(cacheKey), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>()), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(It.IsAny<string>()), Times.Never());
            _inMemoryCacheProviderMock.Verify(imcp => imcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            Assert.Equal(inMemoryValue, obtainedCacheValue);
        }

        [Fact]
        public void GetOrCreateValue_FromDistributedProvider()
        {
            // Arrange
            var cacheKey = "Unit-Test-Cache";
            string inMemoryValue = null;
            var distributedValue = "FromDistributed";
            var providerFunctionValue = "FromProviderFunction";

            _inMemoryCacheProviderMock.Setup(imcp => imcp.Get<string>(cacheKey)).Returns(inMemoryValue);
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(distributedValue);
            string cacheProviderFunc() => providerFunctionValue;

            // Act
            var obtainedCacheValue = _cacheService.GetOrCreateValue(cacheKey, cacheProviderFunc);

            // Assert
            _inMemoryCacheProviderMock.Verify(imcp => imcp.Get<string>(cacheKey), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.GetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey), Times.Once());
            _inMemoryCacheProviderMock.Verify(imcp => imcp.Set(cacheKey, distributedValue), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, It.IsAny<string>()), Times.Never());
            Assert.Equal(distributedValue, obtainedCacheValue);
        }

        [Fact]
        public void GetOrCreateValue_FromProviderFunction()
        {
            // Arrange
            var cacheKey = "Unit-Test-Cache";
            string inMemoryValue = null;
            string distributedValue = null;
            var providerFunctionValue = "FromProviderFunction";

            _inMemoryCacheProviderMock.Setup(imcp => imcp.Get<string>(cacheKey)).Returns(inMemoryValue);
            _distributedCacheProviderMock.Setup(dcp => dcp.Get<string>(cacheKey)).Returns(distributedValue);
            string cacheProviderFunc() => providerFunctionValue;

            // Act
            var obtainedCacheValue = _cacheService.GetOrCreateValue(cacheKey, cacheProviderFunc);

            // Assert
            _inMemoryCacheProviderMock.Verify(imcp => imcp.Get<string>(cacheKey), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.GetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            _distributedCacheProviderMock.Verify(dcp => dcp.Get<string>(cacheKey), Times.Once());
            _inMemoryCacheProviderMock.Verify(imcp => imcp.Set(cacheKey, providerFunctionValue), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.Set(cacheKey, providerFunctionValue), Times.Once());
            Assert.Equal(providerFunctionValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetOrCreateValueAsync_FromInMemoryProvider()
        {
            // Arrange
            var cacheKey = "Unit-Test-Cache";
            var inMemoryValue = "FromInMemory";
            var distributedValue = "FromDistributed";
            var providerFunctionValue = "FromProviderFunction";

            _inMemoryCacheProviderMock.Setup(imcp => imcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>())).Returns(Task.FromResult(inMemoryValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>())).Returns(Task.FromResult(distributedValue));
            string cacheProviderFunc() => providerFunctionValue;

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateValueAsync(cacheKey, cacheProviderFunc);

            // Assert
            _inMemoryCacheProviderMock.Verify(imcp => imcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>()), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>()), Times.Once());
            _inMemoryCacheProviderMock.Verify(imcp => imcp.SetAsync(cacheKey, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            _distributedCacheProviderMock.Verify(dcp => dcp.SetAsync(cacheKey, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            Assert.Equal(inMemoryValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetOrCreateValueAsync_FromDistributedProvider()
        {
            // Arrange
            var cacheKey = "Unit-Test-Cache";
            string inMemoryValue = null;
            var distributedValue = "FromDistributed";
            var providerFunctionValue = "FromProviderFunction";

            _inMemoryCacheProviderMock.Setup(imcp => imcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>())).Returns(Task.FromResult(inMemoryValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>())).Returns(Task.FromResult(distributedValue));
            string cacheProviderFunc() => providerFunctionValue;

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateValueAsync(cacheKey, cacheProviderFunc);

            // Assert
            _inMemoryCacheProviderMock.Verify(imcp => imcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>()), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>()), Times.Once());
            _inMemoryCacheProviderMock.Verify(imcp => imcp.SetAsync(cacheKey, distributedValue, It.IsAny<CancellationToken>()), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.SetAsync(cacheKey, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
            Assert.Equal(distributedValue, obtainedCacheValue);
        }

        [Fact]
        public async void GetOrCreateValueAsync_FromProviderFunction()
        {
            // Arrange
            var cacheKey = "Unit-Test-Cache";
            string inMemoryValue = null;
            string distributedValue = null;
            var providerFunctionValue = "FromProviderFunction";

            _inMemoryCacheProviderMock.Setup(imcp => imcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>())).Returns(Task.FromResult(inMemoryValue));
            _distributedCacheProviderMock.Setup(dcp => dcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>())).Returns(Task.FromResult(distributedValue));
            string cacheProviderFunc() => providerFunctionValue;

            // Act
            var obtainedCacheValue = await _cacheService.GetOrCreateValueAsync(cacheKey, cacheProviderFunc);

            // Assert
            _inMemoryCacheProviderMock.Verify(imcp => imcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>()), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.GetAsync<string>(cacheKey, It.IsAny<CancellationToken>()), Times.Once());
            _inMemoryCacheProviderMock.Verify(imcp => imcp.SetAsync(cacheKey, providerFunctionValue, It.IsAny<CancellationToken>()), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.SetAsync(cacheKey, providerFunctionValue, It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(providerFunctionValue, obtainedCacheValue);
        }

        [Fact]
        public void InvalidateValue()
        {
            // Arrange
            var cacheKey = "Unit-Test-Cache";
            _inMemoryCacheProviderMock.Setup(imcp => imcp.Remove(cacheKey));
            _distributedCacheProviderMock.Setup(dcp => dcp.Remove(cacheKey));

            // Act
            _cacheService.InvalidateValue(cacheKey);

            // Assert
            _inMemoryCacheProviderMock.Verify(imcp => imcp.Remove(cacheKey), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.Remove(cacheKey), Times.Once());
        }

        [Fact]
        public async void InvalidateValueAsync()
        {
            // Arrange
            var cacheKey = "Unit-Test-Cache";
            _inMemoryCacheProviderMock.Setup(imcp => imcp.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()));
            _distributedCacheProviderMock.Setup(dcp => dcp.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()));

            // Act
            await _cacheService.InvalidateValueAsync(cacheKey);

            // Assert
            _inMemoryCacheProviderMock.Verify(imcp => imcp.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once());
            _distributedCacheProviderMock.Verify(dcp => dcp.RemoveAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
