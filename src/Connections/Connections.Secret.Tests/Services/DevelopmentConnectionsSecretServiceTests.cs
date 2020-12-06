namespace Connections.Secret.Tests.Services
{
    using ForexMiner.Heimdallr.Common.Caching.Services;
    using ForexMiner.Heimdallr.Connections.Secret.Services;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class DevelopmentConnectionsSecretServiceTests
    {
        private readonly Mock<ICachingService> _cachingServiceMock;
        private readonly IConnectionsSecretService _connectionsSecretService;

        public DevelopmentConnectionsSecretServiceTests()
        {
            _cachingServiceMock = new Mock<ICachingService>();
            _connectionsSecretService = new DevelopmentConnectionsSecretService(_cachingServiceMock.Object);
        }

        [Fact]
        public async void GetConnectionSecret()
        {
            // Arrange
            var connectionId = Guid.NewGuid();
            var connectionSecretKey = $"ConnectionSecret-{connectionId}";
            var connectionSecretValue = "TopSecret";
            _cachingServiceMock.Setup(cs => cs.GetOrCreateValueAsync(connectionSecretKey, It.IsAny<Func<string>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(connectionSecretValue));

            // Act
            var connectionSecret = await _connectionsSecretService.GetConnectionSecret(connectionId);

            // Assert
            _cachingServiceMock.Verify(cs => cs.GetOrCreateValueAsync(connectionSecretKey, It.IsAny<Func<string>>(), It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(connectionSecretValue, connectionSecret);
        }

        [Fact]
        public async void CreateConnectionSecret()
        {
            // Arrange
            var connectionId = Guid.NewGuid();
            var connectionSecretKey = $"ConnectionSecret-{connectionId}";
            var connectionSecretValue = "TopSecret";
            _cachingServiceMock.Setup(cs => cs.GetOrCreateValueAsync(connectionSecretKey, It.IsAny<Func<string>>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(connectionSecretValue));

            // Act
            await _connectionsSecretService.CreateConnectionSecret(connectionId, connectionSecretValue);

            // Assert
            _cachingServiceMock.Verify(cs => cs.GetOrCreateValueAsync(connectionSecretKey, It.IsAny<Func<string>>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
