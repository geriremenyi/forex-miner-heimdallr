
namespace Connections.Api.Tests.Controllers.V1
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Connection;
    using ForexMiner.Heimdallr.Common.Data.Database.Models.User;
    using ForexMiner.Heimdallr.Connections.Api.Controllers.V1;
    using ForexMiner.Heimdallr.Connections.Api.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Xunit;

    public class ConnectionsControllerTests
    {
        private readonly Mock<IConnectionService> _connectionServiceMock;
        private readonly ConnectionsController _connectionsController;


        public ConnectionsControllerTests()
        {
            _connectionServiceMock = new Mock<IConnectionService>();
            _connectionsController = new ConnectionsController(_connectionServiceMock.Object);
        }

        [Fact]
        public async void GetConnectionsForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUserContext(userId);

            var connectionId = Guid.NewGuid();
            _connectionServiceMock.Setup(cs => cs.GetConnectionsForUser(userId)).Returns(Task.FromResult<IEnumerable<Connection>>(new List<Connection>()
            {
                new Connection()
                { 
                    Id = connectionId,
                }
            }));

            // Act
            var connections = await _connectionsController.GetConnectionsForUser();

            // Assert
            _connectionServiceMock.Verify(cs => cs.GetConnectionsForUser(userId), Times.Once());
            Assert.Single(connections);
            Assert.Equal(connectionId, connections.ElementAt(0).Id);
        }

        [Fact]
        public void TestConnection()
        {
            // Arrange
            var connectionToTest = new ConnectionTest()
            {
                Broker = ForexMiner.Heimdallr.Common.Data.Database.Models.Connection.Broker.Oanda,
                Secret = "SuperSecret"
            };

            var accountIds = new List<string>() { "a", "b", "c" };
            _connectionServiceMock.Setup(cs => cs.TestConnection(connectionToTest)).Returns(new ConnectionTestResults() 
            {
                AccountIds = accountIds,
                Type = ConnectionType.Demo
            });

            // Act
            var testResults =  _connectionsController.TestConnection(connectionToTest);

            // Assert
            _connectionServiceMock.Verify(cs => cs.TestConnection(connectionToTest), Times.Once());
            Assert.Equal(accountIds, testResults.AccountIds);
            Assert.Equal(ConnectionType.Demo, testResults.Type);
        }

        [Fact]
        public async void CreateConnection()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUserContext(userId);

            var connectionToCreate = new ConnectionCreation()
            {
                Broker = ForexMiner.Heimdallr.Common.Data.Database.Models.Connection.Broker.Oanda,
                ExternalAccountId = "1234",
                Name = "Test Broker",
                Secret = "TopSecret"
            };

            var connectionId = Guid.NewGuid();
            _connectionServiceMock.Setup(cs => cs.CreateConnectionForUser(userId, connectionToCreate)).Returns(Task.FromResult(new Connection()
            {
                Id = connectionId
            }));

            // Act
            var connection = await _connectionsController.CreateConnection(connectionToCreate);

            // Assert
            _connectionServiceMock.Verify(cs => cs.CreateConnectionForUser(userId, connectionToCreate), Times.Once());
            Assert.Equal(connectionId, connection.Id);
        }

        private void MockUserContext(Guid userId)
        {
            // Generate claims
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Mock base http context
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(ctx => ctx.User).Returns(claimsPrincipal);

            // Mock controller context
            var controllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };
            _connectionsController.ControllerContext = controllerContext;
        }
    }
}
