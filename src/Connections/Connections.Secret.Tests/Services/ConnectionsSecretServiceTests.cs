namespace Connections.Secret.Tests.Services
{
    using Azure;
    using Azure.Security.KeyVault.Secrets;
    using ForexMiner.Heimdallr.Connections.Secret.Services;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ConnectionsSecretServiceTests
    {
        private readonly Mock<SecretClient> _secretClientMock;
        private readonly IConnectionsSecretService _connectionsSecretService;

        public ConnectionsSecretServiceTests()
        {
            _secretClientMock = new Mock<SecretClient>();
            _connectionsSecretService = new ConnectionsSecretService(_secretClientMock.Object);
        }

        [Fact]
        public async void GetConnectionSecret()
        {
            // Arrange
            var connectionId = Guid.NewGuid();
            var connectionSecretKey = $"ConnectionSecret-{connectionId}";
            var connectionSecretValue = "TopSecret";
            _secretClientMock.Setup(sc => sc.GetSecretAsync(connectionSecretKey, null, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Response.FromValue(new KeyVaultSecret(connectionSecretKey, connectionSecretValue), null)));

            // Act
            var connectionSecret = await _connectionsSecretService.GetConnectionSecret(connectionId);

            // Assert
            _secretClientMock.Verify(sc => sc.GetSecretAsync(connectionSecretKey, null, It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(connectionSecretValue, connectionSecret);
        }

        [Fact]
        public async void CreateConnectionSecret()
        {
            // Arrange
            var connectionId = Guid.NewGuid();
            var connectionSecretKey = $"ConnectionSecret-{connectionId}";
            var connectionSecretValue = "TopSecret";
            _secretClientMock.Setup(sc => sc.SetSecretAsync(connectionSecretKey, connectionSecretValue, It.IsAny<CancellationToken>()));

            // Act
            await _connectionsSecretService.CreateConnectionSecret(connectionId, connectionSecretValue);

            // Assert
            _secretClientMock.Verify(sc => sc.SetSecretAsync(connectionSecretKey, connectionSecretValue, It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
