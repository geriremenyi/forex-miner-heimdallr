namespace Connections.Worker.Tests.Services
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Database = ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument;
    using TradeSignal = ForexMiner.Heimdallr.Common.Data.Database.Models.Trade.TradeSignal;
    using TradeDirection = ForexMiner.Heimdallr.Common.Data.Contracts.Trade.TradeDirection;
    using Connection = ForexMiner.Heimdallr.Common.Data.Database.Models.Connection.Connection;
    using Contracts = ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Common.Data.Mapping;
    using ForexMiner.Heimdallr.Connections.Secret.Services;
    using ForexMiner.Heimdallr.Connections.Worker.Services;
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using GeriRemenyi.Oanda.V20.Sdk;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using System;
    using System.Net.Http;
    using Xunit;
    using System.Collections.Generic;
    using GeriRemenyi.Oanda.V20.Sdk.Common.Types;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using GeriRemenyi.Oanda.V20.Sdk.Instrument;
    using System.Threading;
    using System.Threading.Tasks;
    using GeriRemenyi.Oanda.V20.Sdk.Account;
    using GeriRemenyi.Oanda.V20.Sdk.Trade;
    using Moq.Protected;

    public class TickerServiceTests: IDisposable
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly ForexMinerHeimdallrDbContext _dbContext;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IConnectionsSecretService> _connectionsSecretServiceMock;
        private readonly IMapper _mapper;
        private readonly Mock<IOandaApiConnectionFactory> _oandaApiConnectionFactoryMock;
        private readonly Mock<IInstrumentStorageService> _instrumentStorageServiceMock;
        private readonly ITickerService _tickerService;

        public TickerServiceTests()
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
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Max-Retries")]).Returns("3");
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _connectionsSecretServiceMock = new Mock<IConnectionsSecretService>();
            _oandaApiConnectionFactoryMock = new Mock<IOandaApiConnectionFactory>();
            _instrumentStorageServiceMock = new Mock<IInstrumentStorageService>();

            // Class under test
            _tickerService = new TickerService(
                _configurationMock.Object,
                _dbContext,
                _httpClientFactoryMock.Object,
                _connectionsSecretServiceMock.Object,
                _mapper,
                _oandaApiConnectionFactoryMock.Object,
                _instrumentStorageServiceMock.Object
            );
        }

        [Fact]
        public async void Tick()
        {
            // Arrange
            var instrument = new Database.Instrument()
            {
                Name = Contracts.InstrumentName.EUR_USD
            };
            var granularity = new Database.InstrumentGranularity()
            {
                Id = Guid.NewGuid(),
                Instrument = instrument,
                Granularity = Contracts.Granularity.M1,
                State = Database.GranularityState.Tradeable
            };
            instrument.Granularities.Add(granularity);
            _dbContext.Add(instrument);
            _dbContext.SaveChanges();

            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "Oanda-MasterToken")]).Returns("TopSecretOandaToken");
            var oandaLastTwoCandles = new List<Candlestick>()
            {
                new Candlestick()
                {
                    Time = DateTime.UtcNow.AddMinutes(-1).ToShortDateString(),
                    Volume = 12345,
                    Complete = true,
                    Bid = new CandlestickData()
                    { 
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Mid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Ask = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    }
                },
                new Candlestick()
                {
                    Time = DateTime.UtcNow.ToShortDateString(),
                    Volume = 12345,
                    Complete = false,
                    Bid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Mid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Ask = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    }
                }
            };
            var oandaInstrumentMock = new Mock<IInstrument>();
            oandaInstrumentMock.Setup(oi => oi.GetLastNCandlesAsync(
                CandlestickGranularity.M1,
                2,
                It.IsAny<IEnumerable<PricingComponent>>()
            )).Returns(Task.FromResult<IEnumerable<Candlestick>>(oandaLastTwoCandles));
            var oandaConnectionMock = new Mock<IOandaApiConnection>();
            oandaConnectionMock.Setup(oc => oc.GetInstrument(It.IsAny<InstrumentName>())).Returns(oandaInstrumentMock.Object);
            _oandaApiConnectionFactoryMock.Setup(oacf => oacf.CreateConnection(It.IsAny<OandaConnectionType>(), It.IsAny<string>(), It.IsAny<DateTimeFormat>())).Returns(oandaConnectionMock.Object);

            _instrumentStorageServiceMock.Setup(iss => iss.StoreInstrumentCandles(It.IsAny<Contracts.InstrumentWithCandles>()));

            var thorHttpClientFactoryName = "LokiIsSoOverrated";
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Name")]).Returns(thorHttpClientFactoryName);
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Content-Type")]).Returns("application/json");
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var thorResponse = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(GetSerializedTradeSignal(instrument.Name))
            };
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(thorResponse);
            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            { 
                BaseAddress = new Uri("https://this-must-be-thor.ai")
            };
            _httpClientFactoryMock.Setup(hcf => hcf.CreateClient(thorHttpClientFactoryName)).Returns(httpClient);

            var connection = new Connection()
            {
                Id = Guid.NewGuid(),
                Broker = ForexMiner.Heimdallr.Common.Data.Database.Models.Connection.Broker.Oanda,
                ExternalAccountId = "123456",
                Name = "This is a test connection",
                Owner = null,
                Type = ForexMiner.Heimdallr.Common.Data.Contracts.Connection.ConnectionType.Demo
            };
            _dbContext.Add(connection);
            _dbContext.SaveChanges();

            _connectionsSecretServiceMock.Setup(css => css.GetConnectionSecret(It.IsAny<Guid>()));
            var oandaAccountMock = new Mock<IAccount>();
            oandaAccountMock.Setup(oa => oa.GetDetailsAsync()).Returns(Task.FromResult(
                new GeriRemenyi.Oanda.V20.Client.Model.Account()
                { 
                    Balance = 1000,
                    UnrealizedPL = 100
                }
            ));
            var oandaTradesMock = new Mock<ITrades>();
            oandaTradesMock.Setup(ot => ot.OpenTradeAsync(
                It.IsAny<InstrumentName>(),
                It.IsAny<GeriRemenyi.Oanda.V20.Sdk.Trade.TradeDirection>(),
                It.IsAny<long>(),
                It.IsAny<int>()
            ));
            oandaAccountMock.SetupGet(oa => oa.Trades).Returns(oandaTradesMock.Object);
            oandaConnectionMock.Setup(oc => oc.GetAccount(It.IsAny<string>())).Returns(oandaAccountMock.Object);

            // Act
            await _tickerService.Tick();

            // Assert
            oandaInstrumentMock.Verify(oi => oi.GetLastNCandlesAsync(CandlestickGranularity.M1, 2, It.IsAny<IEnumerable<PricingComponent>>()), Times.Once());
            _instrumentStorageServiceMock.Verify(iss => iss.StoreInstrumentCandles(It.IsAny<Contracts.InstrumentWithCandles>()), Times.Once());
            _httpClientFactoryMock.Verify(hcf => hcf.CreateClient(thorHttpClientFactoryName), Times.Once());
            _connectionsSecretServiceMock.Verify(css => css.GetConnectionSecret(It.IsAny<Guid>()), Times.Once());
            oandaTradesMock.Verify(ot => ot.OpenTradeAsync(
                It.IsAny<InstrumentName>(),
                It.IsAny<GeriRemenyi.Oanda.V20.Sdk.Trade.TradeDirection>(),
                It.IsAny<long>(),
                It.IsAny<int>()
            ), Times.Once());
        }

        [Fact]
        public async void Tick_NoTradeableInstruments()
        {
            // Arrange
            var instrument = new Database.Instrument()
            {
                Name = Contracts.InstrumentName.EUR_USD
            };
            var granularity = new Database.InstrumentGranularity()
            {
                Id = Guid.NewGuid(),
                Instrument = instrument,
                Granularity = Contracts.Granularity.M1,
                State = Database.GranularityState.New
            };
            instrument.Granularities.Add(granularity);
            _dbContext.Add(instrument);
            _dbContext.SaveChanges();

            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "Oanda-MasterToken")]).Returns("TopSecretOandaToken");
            var oandaLastTwoCandles = new List<Candlestick>()
            {
                new Candlestick()
                {
                    Time = DateTime.UtcNow.AddMinutes(-1).ToShortDateString(),
                    Volume = 12345,
                    Complete = true,
                    Bid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Mid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Ask = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    }
                },
                new Candlestick()
                {
                    Time = DateTime.UtcNow.ToShortDateString(),
                    Volume = 12345,
                    Complete = false,
                    Bid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Mid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Ask = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    }
                }
            };
            var oandaInstrumentMock = new Mock<IInstrument>();
            oandaInstrumentMock.Setup(oi => oi.GetLastNCandlesAsync(
                CandlestickGranularity.M1,
                2,
                It.IsAny<IEnumerable<PricingComponent>>()
            )).Returns(Task.FromResult<IEnumerable<Candlestick>>(oandaLastTwoCandles));
            var oandaConnectionMock = new Mock<IOandaApiConnection>();
            oandaConnectionMock.Setup(oc => oc.GetInstrument(It.IsAny<InstrumentName>())).Returns(oandaInstrumentMock.Object);
            _oandaApiConnectionFactoryMock.Setup(oacf => oacf.CreateConnection(It.IsAny<OandaConnectionType>(), It.IsAny<string>(), It.IsAny<DateTimeFormat>())).Returns(oandaConnectionMock.Object);

            _instrumentStorageServiceMock.Setup(iss => iss.StoreInstrumentCandles(It.IsAny<Contracts.InstrumentWithCandles>()));

            var thorHttpClientFactoryName = "LokiIsSoOverrated";
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Name")]).Returns(thorHttpClientFactoryName);
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Content-Type")]).Returns("application/json");
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var thorResponse = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(GetSerializedTradeSignal(instrument.Name))
            };
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(thorResponse);
            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://this-must-be-thor.ai")
            };
            _httpClientFactoryMock.Setup(hcf => hcf.CreateClient(thorHttpClientFactoryName)).Returns(httpClient);

            var connection = new Connection()
            {
                Id = Guid.NewGuid(),
                Broker = ForexMiner.Heimdallr.Common.Data.Database.Models.Connection.Broker.Oanda,
                ExternalAccountId = "123456",
                Name = "This is a test connection",
                Owner = null,
                Type = ForexMiner.Heimdallr.Common.Data.Contracts.Connection.ConnectionType.Demo
            };
            _dbContext.Add(connection);
            _dbContext.SaveChanges();

            var oandaAccountMock = new Mock<IAccount>();
            oandaAccountMock.Setup(oa => oa.GetDetailsAsync()).Returns(Task.FromResult(
                new GeriRemenyi.Oanda.V20.Client.Model.Account()
                {
                    Balance = 1000,
                    UnrealizedPL = 100
                }
            ));
            var oandaTradesMock = new Mock<ITrades>();
            oandaTradesMock.Setup(ot => ot.OpenTradeAsync(
                It.IsAny<InstrumentName>(),
                It.IsAny<GeriRemenyi.Oanda.V20.Sdk.Trade.TradeDirection>(),
                It.IsAny<long>(),
                It.IsAny<int>()
            ));
            oandaAccountMock.SetupGet(oa => oa.Trades).Returns(oandaTradesMock.Object);
            oandaConnectionMock.Setup(oc => oc.GetAccount(It.IsAny<string>())).Returns(oandaAccountMock.Object);

            // Act
            await _tickerService.Tick();

            // Assert
            oandaInstrumentMock.Verify(oi => oi.GetLastNCandlesAsync(CandlestickGranularity.M1, 2, It.IsAny<IEnumerable<PricingComponent>>()), Times.Never());
            _instrumentStorageServiceMock.Verify(iss => iss.StoreInstrumentCandles(It.IsAny<Contracts.InstrumentWithCandles>()), Times.Never());
            _httpClientFactoryMock.Verify(hcf => hcf.CreateClient(thorHttpClientFactoryName), Times.Never());
            _connectionsSecretServiceMock.Verify(css => css.GetConnectionSecret(It.IsAny<Guid>()), Times.Never());
            oandaTradesMock.Verify(ot => ot.OpenTradeAsync(
                It.IsAny<InstrumentName>(),
                It.IsAny<GeriRemenyi.Oanda.V20.Sdk.Trade.TradeDirection>(),
                It.IsAny<long>(),
                It.IsAny<int>()
            ), Times.Never());
        }

        [Fact]
        public async void Tick_NoTradeSignals()
        {
            // Arrange
            var instrument = new Database.Instrument()
            {
                Name = Contracts.InstrumentName.EUR_USD
            };
            var granularity = new Database.InstrumentGranularity()
            {
                Id = Guid.NewGuid(),
                Instrument = instrument,
                Granularity = Contracts.Granularity.M1,
                State = Database.GranularityState.Tradeable
            };
            instrument.Granularities.Add(granularity);
            _dbContext.Add(instrument);
            _dbContext.SaveChanges();

            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "Oanda-MasterToken")]).Returns("TopSecretOandaToken");
            var oandaLastTwoCandles = new List<Candlestick>()
            {
                new Candlestick()
                {
                    Time = DateTime.UtcNow.AddMinutes(-1).ToShortDateString(),
                    Volume = 12345,
                    Complete = true,
                    Bid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Mid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Ask = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    }
                },
                new Candlestick()
                {
                    Time = DateTime.UtcNow.ToShortDateString(),
                    Volume = 12345,
                    Complete = false,
                    Bid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Mid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Ask = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    }
                }
            };
            var oandaInstrumentMock = new Mock<IInstrument>();
            oandaInstrumentMock.Setup(oi => oi.GetLastNCandlesAsync(
                CandlestickGranularity.M1,
                2,
                It.IsAny<IEnumerable<PricingComponent>>()
            )).Returns(Task.FromResult<IEnumerable<Candlestick>>(oandaLastTwoCandles));
            var oandaConnectionMock = new Mock<IOandaApiConnection>();
            oandaConnectionMock.Setup(oc => oc.GetInstrument(It.IsAny<InstrumentName>())).Returns(oandaInstrumentMock.Object);
            _oandaApiConnectionFactoryMock.Setup(oacf => oacf.CreateConnection(It.IsAny<OandaConnectionType>(), It.IsAny<string>(), It.IsAny<DateTimeFormat>())).Returns(oandaConnectionMock.Object);

            _instrumentStorageServiceMock.Setup(iss => iss.StoreInstrumentCandles(It.IsAny<Contracts.InstrumentWithCandles>()));

            var thorHttpClientFactoryName = "LokiIsSoOverrated";
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Name")]).Returns(thorHttpClientFactoryName);
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Content-Type")]).Returns("application/json");
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var thorResponse = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new List<TradeSignal>()))
            };
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(thorResponse);
            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://this-must-be-thor.ai")
            };
            _httpClientFactoryMock.Setup(hcf => hcf.CreateClient(thorHttpClientFactoryName)).Returns(httpClient);

            var connection = new Connection()
            {
                Id = Guid.NewGuid(),
                Broker = ForexMiner.Heimdallr.Common.Data.Database.Models.Connection.Broker.Oanda,
                ExternalAccountId = "123456",
                Name = "This is a test connection",
                Owner = null,
                Type = ForexMiner.Heimdallr.Common.Data.Contracts.Connection.ConnectionType.Demo
            };
            _dbContext.Add(connection);
            _dbContext.SaveChanges();


            _connectionsSecretServiceMock.Setup(css => css.GetConnectionSecret(It.IsAny<Guid>()));
            var oandaAccountMock = new Mock<IAccount>();
            oandaAccountMock.Setup(oa => oa.GetDetailsAsync()).Returns(Task.FromResult(
                new GeriRemenyi.Oanda.V20.Client.Model.Account()
                {
                    Balance = 1000,
                    UnrealizedPL = 100
                }
            ));
            var oandaTradesMock = new Mock<ITrades>();
            oandaTradesMock.Setup(ot => ot.OpenTradeAsync(
                It.IsAny<InstrumentName>(),
                It.IsAny<GeriRemenyi.Oanda.V20.Sdk.Trade.TradeDirection>(),
                It.IsAny<long>(),
                It.IsAny<int>()
            ));
            oandaAccountMock.SetupGet(oa => oa.Trades).Returns(oandaTradesMock.Object);
            oandaConnectionMock.Setup(oc => oc.GetAccount(It.IsAny<string>())).Returns(oandaAccountMock.Object);

            // Act
            await _tickerService.Tick();

            // Assert
            oandaInstrumentMock.Verify(oi => oi.GetLastNCandlesAsync(CandlestickGranularity.M1, 2, It.IsAny<IEnumerable<PricingComponent>>()), Times.Once());
            _instrumentStorageServiceMock.Verify(iss => iss.StoreInstrumentCandles(It.IsAny<Contracts.InstrumentWithCandles>()), Times.Once());
            _httpClientFactoryMock.Verify(hcf => hcf.CreateClient(thorHttpClientFactoryName), Times.Once());
            _connectionsSecretServiceMock.Verify(css => css.GetConnectionSecret(It.IsAny<Guid>()), Times.Once());
            oandaTradesMock.Verify(ot => ot.OpenTradeAsync(
                It.IsAny<InstrumentName>(),
                It.IsAny<GeriRemenyi.Oanda.V20.Sdk.Trade.TradeDirection>(),
                It.IsAny<long>(),
                It.IsAny<int>()
            ), Times.Never());
        }

        [Fact]
        public async void Tick_NoConnections()
        {
            // Arrange
            var instrument = new Database.Instrument()
            {
                Name = Contracts.InstrumentName.EUR_USD
            };
            var granularity = new Database.InstrumentGranularity()
            {
                Id = Guid.NewGuid(),
                Instrument = instrument,
                Granularity = Contracts.Granularity.M1,
                State = Database.GranularityState.Tradeable
            };
            instrument.Granularities.Add(granularity);
            _dbContext.Add(instrument);
            _dbContext.SaveChanges();

            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "Oanda-MasterToken")]).Returns("TopSecretOandaToken");
            var oandaLastTwoCandles = new List<Candlestick>()
            {
                new Candlestick()
                {
                    Time = DateTime.UtcNow.AddMinutes(-1).ToShortDateString(),
                    Volume = 12345,
                    Complete = true,
                    Bid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Mid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Ask = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    }
                },
                new Candlestick()
                {
                    Time = DateTime.UtcNow.ToShortDateString(),
                    Volume = 12345,
                    Complete = false,
                    Bid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Mid = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    },
                    Ask = new CandlestickData()
                    {
                        O = 1.2345,
                        H = 1.2345,
                        L = 1.2345,
                        C = 1.2345
                    }
                }
            };
            var oandaInstrumentMock = new Mock<IInstrument>();
            oandaInstrumentMock.Setup(oi => oi.GetLastNCandlesAsync(
                CandlestickGranularity.M1,
                2,
                It.IsAny<IEnumerable<PricingComponent>>()
            )).Returns(Task.FromResult<IEnumerable<Candlestick>>(oandaLastTwoCandles));
            var oandaConnectionMock = new Mock<IOandaApiConnection>();
            oandaConnectionMock.Setup(oc => oc.GetInstrument(It.IsAny<InstrumentName>())).Returns(oandaInstrumentMock.Object);
            _oandaApiConnectionFactoryMock.Setup(oacf => oacf.CreateConnection(It.IsAny<OandaConnectionType>(), It.IsAny<string>(), It.IsAny<DateTimeFormat>())).Returns(oandaConnectionMock.Object);

            _instrumentStorageServiceMock.Setup(iss => iss.StoreInstrumentCandles(It.IsAny<Contracts.InstrumentWithCandles>()));

            var thorHttpClientFactoryName = "LokiIsSoOverrated";
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Name")]).Returns(thorHttpClientFactoryName);
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Content-Type")]).Returns("application/json");
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var thorResponse = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(GetSerializedTradeSignal(instrument.Name))
            };
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(thorResponse);
            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://this-must-be-thor.ai")
            };
            _httpClientFactoryMock.Setup(hcf => hcf.CreateClient(thorHttpClientFactoryName)).Returns(httpClient);

            _connectionsSecretServiceMock.Setup(css => css.GetConnectionSecret(It.IsAny<Guid>()));
            var oandaAccountMock = new Mock<IAccount>();
            oandaAccountMock.Setup(oa => oa.GetDetailsAsync()).Returns(Task.FromResult(
                new GeriRemenyi.Oanda.V20.Client.Model.Account()
                {
                    Balance = 1000,
                    UnrealizedPL = 100
                }
            ));
            var oandaTradesMock = new Mock<ITrades>();
            oandaTradesMock.Setup(ot => ot.OpenTradeAsync(
                It.IsAny<InstrumentName>(),
                It.IsAny<GeriRemenyi.Oanda.V20.Sdk.Trade.TradeDirection>(),
                It.IsAny<long>(),
                It.IsAny<int>()
            ));
            oandaAccountMock.SetupGet(oa => oa.Trades).Returns(oandaTradesMock.Object);
            oandaConnectionMock.Setup(oc => oc.GetAccount(It.IsAny<string>())).Returns(oandaAccountMock.Object);

            // Act
            await _tickerService.Tick();

            // Assert
            oandaInstrumentMock.Verify(oi => oi.GetLastNCandlesAsync(CandlestickGranularity.M1, 2, It.IsAny<IEnumerable<PricingComponent>>()), Times.Once());
            _instrumentStorageServiceMock.Verify(iss => iss.StoreInstrumentCandles(It.IsAny<Contracts.InstrumentWithCandles>()), Times.Once());
            _httpClientFactoryMock.Verify(hcf => hcf.CreateClient(thorHttpClientFactoryName), Times.Once());
            _connectionsSecretServiceMock.Verify(css => css.GetConnectionSecret(It.IsAny<Guid>()), Times.Never());
            oandaTradesMock.Verify(ot => ot.OpenTradeAsync(
                It.IsAny<InstrumentName>(),
                It.IsAny<GeriRemenyi.Oanda.V20.Sdk.Trade.TradeDirection>(),
                It.IsAny<long>(),
                It.IsAny<int>()
            ), Times.Never());
        }

        private string GetSerializedTradeSignal(Contracts.InstrumentName instrument)
        {
            return System.Text.Json.JsonSerializer.Serialize(new List<TradeSignal>()
            { 
                new TradeSignal()
                { 
                    Id = Guid.NewGuid(),
                    Instrument = instrument,
                    Direction = TradeDirection.Long,
                    Confidence = 1.0,
                    Time = DateTime.UtcNow
                }
            });
        }

        public void Dispose()
        {
            _dbContext.Database.CloseConnection();
        }
    }
}
