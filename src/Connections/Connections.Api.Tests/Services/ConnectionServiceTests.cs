namespace Connections.Api.Tests.Services
{
    using Database = ForexMiner.Heimdallr.Common.Data.Database.Models.Connection;
    using Contracts = ForexMiner.Heimdallr.Common.Data.Contracts.Connection;
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using ForexMiner.Heimdallr.Common.Data.Mapping;
    using ForexMiner.Heimdallr.Connections.Api.Services;
    using ForexMiner.Heimdallr.Connections.Secret.Services;
    using GeriRemenyi.Oanda.V20.Sdk;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using System;
    using Xunit;
    using System.Collections.Generic;
    using GeriRemenyi.Oanda.V20.Sdk.Account;
    using System.Threading.Tasks;
    using GeriRemenyi.Oanda.V20.Sdk.Trade;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using System.Linq;
    using ForexMiner.Heimdallr.Common.Data.Database.Models.User;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Connection;
    using ForexMiner.Heimdallr.Common.Data.Exceptions;
    using System.Net;

    public class ConnectionServiceTests : IDisposable
    {
        private readonly ForexMinerHeimdallrDbContext _dbContext;
        private readonly Mock<IConnectionsSecretService> _connectionsSecretServiceMock;
        private readonly IMapper _mapper;
        private readonly Mock<IOandaApiConnectionFactory> _oandaApiFactoryMock;
        private readonly IConnectionService _connectionService;

        public ConnectionServiceTests()
        {
            // Setup an actual in-memory Sqlite for db mocking
            var optionsBuilder = new DbContextOptionsBuilder<ForexMinerHeimdallrDbContext>();
            optionsBuilder.UseSqlite("Filename=:memory:");
            _dbContext = new ForexMinerHeimdallrDbContext(optionsBuilder.Options);
            _dbContext.Database.OpenConnection();
            _dbContext.Database.Migrate();

            // Auto mapper
            var contractContract = new ContractContractMappings();
            var databaseContract = new DatabaseContractMappings();
            var oandaContract = new OandaContractMappings();
            var configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile(contractContract);
                cfg.AddProfile(databaseContract);
                cfg.AddProfile(oandaContract);
            });
            _mapper = new Mapper(configuration);

            // Mocks
            _connectionsSecretServiceMock = new Mock<IConnectionsSecretService>();
            _oandaApiFactoryMock = new Mock<IOandaApiConnectionFactory>();

            // Class under test
            _connectionService = new ConnectionService(_dbContext, _connectionsSecretServiceMock.Object, _mapper, _oandaApiFactoryMock.Object);
        }

        [Fact]
        public async void GetConnectionsForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User()
            {
                Id = userId,
                Email = "test@test.ai",
                FirstName = "Elek",
                LastName = "Test",
                Role = Role.Trader
            };
            user.UpdatePassword("SuperSecretivePassword");
            var connetion = new Database.Connection()
            {
                Id = Guid.NewGuid(),
                Broker = Database.Broker.Oanda,
                Name = "Test Oanda Connection",
                ExternalAccountId = "1234",
                Type = Contracts.ConnectionType.Demo
            };
            user.Connections.Add(connetion);
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            var connectionSecret = "MuchSecretSoHidden";
            _connectionsSecretServiceMock.Setup(css => css.GetConnectionSecret(connetion.Id)).Returns(Task.FromResult(connectionSecret));

            var oandaTradesMock = new Mock<ITrades>();
            oandaTradesMock.Setup(ot => ot.GetOpenTradesAsync())
                .Returns(Task.FromResult<IEnumerable<GeriRemenyi.Oanda.V20.Client.Model.Trade>>(new List<GeriRemenyi.Oanda.V20.Client.Model.Trade>()
                {
                    new GeriRemenyi.Oanda.V20.Client.Model.Trade()
                    {
                        Id = 1234,
                        Instrument = GeriRemenyi.Oanda.V20.Client.Model.InstrumentName.EUR_USD,
                        Price = 1.2345,
                        OpenTime = DateTime.UtcNow.ToShortDateString(),
                        CurrentUnits = 10
                    }
                }));
            var account = new GeriRemenyi.Oanda.V20.Client.Model.Account()
            {
                Balance = 9999999999,
                Pl = 100
            };
            var oandaAccountMock = new Mock<IAccount>();
            oandaAccountMock.Setup(oa => oa.GetDetailsAsync()).Returns(Task.FromResult(account));
            oandaAccountMock.SetupGet(oa => oa.Trades).Returns(oandaTradesMock.Object);
            var oandaConnectionMock = new Mock<IOandaApiConnection>();
            oandaConnectionMock.Setup(oc => oc.GetAccount(connetion.ExternalAccountId)).Returns(oandaAccountMock.Object);
            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, connectionSecret, It.IsAny<DateTimeFormat>()))
                .Returns(oandaConnectionMock.Object);


            // Act
            var userConnections = await _connectionService.GetConnectionsForUser(userId);

            // Assert
            _oandaApiFactoryMock.Verify(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, connectionSecret, It.IsAny<DateTimeFormat>()), Times.Once());
            oandaConnectionMock.Verify(oc => oc.GetAccount(connetion.ExternalAccountId), Times.Once());
            oandaAccountMock.Verify(oa => oa.GetDetailsAsync(), Times.Once());
            oandaAccountMock.VerifyGet(oa => oa.Trades, Times.Once());
            oandaTradesMock.Verify(ot => ot.GetOpenTradesAsync(), Times.Once());
            Assert.Single(userConnections);
            Assert.Equal(account.Balance, userConnections.ElementAt(0).Balance);
            Assert.Equal(account.Pl, userConnections.ElementAt(0).ProfitLoss);
        }

        [Fact]
        public void TestConnection_Demo()
        {
            // Arrange
            var secret = "TopSecret";
            var accountId = "12345";
            var connectionToTest = new ConnectionTest()
            {
                Broker = Database.Broker.Oanda,
                Secret = secret
            };

            var oandaConnectionMock = new Mock<IOandaApiConnection>();
            oandaConnectionMock.Setup(oc => oc.GetAccounts()).Returns(new List<AccountProperties>()
            { 
                new AccountProperties()
                { 
                    Id = accountId
                }
            });
            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, secret, It.IsAny<DateTimeFormat>()))
               .Returns(oandaConnectionMock.Object);
            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxTrade, secret, It.IsAny<DateTimeFormat>()))
               .Returns(oandaConnectionMock.Object);

            // Act
            var testResults = _connectionService.TestConnection(connectionToTest);

            // Assert
            _oandaApiFactoryMock.Verify(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, secret, It.IsAny<DateTimeFormat>()), Times.Once());
            _oandaApiFactoryMock.Verify(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxTrade, secret, It.IsAny<DateTimeFormat>()), Times.Never());
            oandaConnectionMock.Verify(oc => oc.GetAccounts(), Times.Once());
            Assert.Equal(ConnectionType.Demo, testResults.Type);
            Assert.Contains(accountId, testResults.AccountIds);
        }

        [Fact]
        public void TestConnection_Live()
        {
            // Arrange
            var secret = "TopSecret";
            var accountId = "12345";
            var connectionToTest = new ConnectionTest()
            {
                Broker = Database.Broker.Oanda,
                Secret = secret
            };

            var oandaConnectionMock = new Mock<IOandaApiConnection>();
            oandaConnectionMock.Setup(oc => oc.GetAccounts()).Returns(new List<AccountProperties>()
            {
                new AccountProperties()
                {
                    Id = accountId
                }
            });
            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, secret, It.IsAny<DateTimeFormat>()))
               .Throws(new Exception());
            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxTrade, secret, It.IsAny<DateTimeFormat>()))
               .Returns(oandaConnectionMock.Object);

            // Act
            var testResults = _connectionService.TestConnection(connectionToTest);

            // Assert
            _oandaApiFactoryMock.Verify(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, secret, It.IsAny<DateTimeFormat>()), Times.Once());
            _oandaApiFactoryMock.Verify(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxTrade, secret, It.IsAny<DateTimeFormat>()), Times.Once());
            oandaConnectionMock.Verify(oc => oc.GetAccounts(), Times.Once());
            Assert.Equal(ConnectionType.Live, testResults.Type);
            Assert.Contains(accountId, testResults.AccountIds);
        }

        [Fact]
        public void TestConnection_Invalid()
        {
            // Arrange
            var secret = "TopSecret";
            var connectionToTest = new ConnectionTest()
            {
                Broker = Database.Broker.Oanda,
                Secret = secret
            };

            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, secret, It.IsAny<DateTimeFormat>()))
               .Throws(new Exception());
            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxTrade, secret, It.IsAny<DateTimeFormat>()))
               .Throws(new Exception());

            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _connectionService.TestConnection(connectionToTest));
            _oandaApiFactoryMock.Verify(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, secret, It.IsAny<DateTimeFormat>()), Times.Once());
            _oandaApiFactoryMock.Verify(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxTrade, secret, It.IsAny<DateTimeFormat>()), Times.Once());
            Assert.Equal(HttpStatusCode.BadRequest, exception.Status);
        }

        [Fact]
        public void CreateConnection()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User()
            {
                Id = userId,
                Email = "test@test.ai",
                FirstName = "Elek",
                LastName = "Test",
                Role = Role.Trader
            };
            user.UpdatePassword("SuperSecretivePassword");
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            var connectionToCreate = new ConnectionCreation()
            {
                Broker = Database.Broker.Oanda,
                ExternalAccountId = "1234",
                Name = "Test Connection",
                Secret = "TopSecret"
            };

            var oandaConnectionMock = new Mock<IOandaApiConnection>();
            oandaConnectionMock.Setup(oc => oc.GetAccounts()).Returns(new List<AccountProperties>()
            {
                new AccountProperties()
                {
                    Id = connectionToCreate.ExternalAccountId
                }
            });
            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, connectionToCreate.Secret, It.IsAny<DateTimeFormat>()))
               .Returns(oandaConnectionMock.Object);

            _connectionsSecretServiceMock.Setup(css => css.CreateConnectionSecret(It.IsAny<Guid>(), connectionToCreate.Secret));

            // Act
            _connectionService.CreateConnectionForUser(userId, connectionToCreate);

            // Assert
            Assert.Single(_dbContext.Connections);
            Assert.Equal(connectionToCreate.Broker, _dbContext.Connections.ToList().ElementAt(0).Broker);
            Assert.Equal(connectionToCreate.ExternalAccountId, _dbContext.Connections.ToList().ElementAt(0).ExternalAccountId);
            Assert.Equal(connectionToCreate.Name, _dbContext.Connections.ToList().ElementAt(0).Name);
            _connectionsSecretServiceMock.Verify(css => css.CreateConnectionSecret(It.IsAny<Guid>(), connectionToCreate.Secret), Times.Once());
        }

        [Fact]
        public async void CreateConnection_Invalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User()
            {
                Id = userId,
                Email = "test@test.ai",
                FirstName = "Elek",
                LastName = "Test",
                Role = Role.Trader
            };
            user.UpdatePassword("SuperSecretivePassword");
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            var connectionToCreate = new ConnectionCreation()
            {
                Broker = Database.Broker.Oanda,
                ExternalAccountId = "1234",
                Name = "Test Connection",
                Secret = "TopSecret"
            };

            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, connectionToCreate.Secret, It.IsAny<DateTimeFormat>()))
               .Throws(new Exception());
            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxTrade, connectionToCreate.Secret, It.IsAny<DateTimeFormat>()))
               .Throws(new Exception());

            _connectionsSecretServiceMock.Setup(css => css.CreateConnectionSecret(It.IsAny<Guid>(), connectionToCreate.Secret));

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<ProblemDetailsException>(async () => await _connectionService.CreateConnectionForUser(userId, connectionToCreate));
            Assert.Equal(HttpStatusCode.BadRequest, exception.Status);
            Assert.Empty(_dbContext.Connections);
            _connectionsSecretServiceMock.Verify(css => css.CreateConnectionSecret(It.IsAny<Guid>(), connectionToCreate.Secret), Times.Never());
        }

        [Fact]
        public async void CreateConnection_Invalid_AccountId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User()
            {
                Id = userId,
                Email = "test@test.ai",
                FirstName = "Elek",
                LastName = "Test",
                Role = Role.Trader
            };
            user.UpdatePassword("SuperSecretivePassword");
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            var connectionToCreate = new ConnectionCreation()
            {
                Broker = Database.Broker.Oanda,
                ExternalAccountId = "1234",
                Name = "Test Connection",
                Secret = "TopSecret"
            };

            var oandaConnectionMock = new Mock<IOandaApiConnection>();
            oandaConnectionMock.Setup(oc => oc.GetAccounts()).Returns(new List<AccountProperties>()
            {
                new AccountProperties()
                {
                    Id = "4321"
                }
            });
            _oandaApiFactoryMock.Setup(ocf => ocf.CreateConnection(GeriRemenyi.Oanda.V20.Sdk.Common.Types.OandaConnectionType.FxPractice, connectionToCreate.Secret, It.IsAny<DateTimeFormat>()))
               .Returns(oandaConnectionMock.Object);

            _connectionsSecretServiceMock.Setup(css => css.CreateConnectionSecret(It.IsAny<Guid>(), connectionToCreate.Secret));

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<ProblemDetailsException>(async () => await _connectionService.CreateConnectionForUser(userId, connectionToCreate));
            Assert.Equal(HttpStatusCode.BadRequest, exception.Status);
            Assert.Empty(_dbContext.Connections);
            _connectionsSecretServiceMock.Verify(css => css.CreateConnectionSecret(It.IsAny<Guid>(), connectionToCreate.Secret), Times.Never());
        }

        public void Dispose()
        {
            _dbContext.Database.CloseConnection();
        }
    }
}
