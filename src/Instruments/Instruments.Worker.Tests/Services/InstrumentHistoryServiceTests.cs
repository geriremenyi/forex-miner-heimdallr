namespace Instruments.Worker.Tests.Services
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Database = ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument;
    using ForexMiner.Heimdallr.Common.Data.Mapping;
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using ForexMiner.Heimdallr.Instruments.Worker.Services;
    using ClientModel = GeriRemenyi.Oanda.V20.Client.Model;
    using GeriRemenyi.Oanda.V20.Sdk;
    using GeriRemenyi.Oanda.V20.Sdk.Common.Types;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using System.Net.Http;
    using Xunit;
    using System.Collections.Generic;
    using System;
    using GeriRemenyi.Oanda.V20.Sdk.Instrument;
    using System.Threading.Tasks;

    public class InstrumentHistoryServiceTests: IDisposable
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly ForexMinerHeimdallrDbContext _dbContext;
        private readonly Mock<IInstrumentStorageService> _instrumentStorageServiceMock;
        private readonly IMapper _mapper;
        private readonly Mock<IOandaApiConnectionFactory> _oandaApiConnectionFactoryMock;
        private readonly IInstrumentHistoryService _instrumentHistoryService;

        public InstrumentHistoryServiceTests()
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
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _instrumentStorageServiceMock = new Mock<IInstrumentStorageService>();
            _oandaApiConnectionFactoryMock = new Mock<IOandaApiConnectionFactory>();
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Max-Retries")]).Returns("5");
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Name")]).Returns("forex-miner-thor");
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "forex-miner-thor:Content-Type")]).Returns("application/json");
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "Oanda-MasterToken")]).Returns("FakeToken");

            // Class under test
            _instrumentHistoryService = new InstrumentHistoryService(
                _configurationMock.Object,
                _httpClientFactoryMock.Object,
                _dbContext,
                _instrumentStorageServiceMock.Object,
                _mapper,
                _oandaApiConnectionFactoryMock.Object
            );
        }

        [Fact]
        public async void CheckInstrumentGranularitiesAndLoadData_NoNewGranularities()
        {
            // Arrange
            _oandaApiConnectionFactoryMock.Setup(oacf => oacf.CreateConnection(It.IsAny<OandaConnectionType>(), It.IsAny<string>(), It.IsAny<ClientModel.DateTimeFormat>()));
            _httpClientFactoryMock.Setup(hcf => hcf.CreateClient("forex-miner-thor")).Returns(new Mock<HttpClient>().Object);
            _instrumentStorageServiceMock.Setup(iss => iss.StoreInstrumentCandles(It.IsAny<InstrumentWithCandles>()));

            // Act
            await _instrumentHistoryService.CheckInstrumentGranularitiesAndLoadData();

            // Assert
            _oandaApiConnectionFactoryMock.Verify(oacf => oacf.CreateConnection(It.IsAny<OandaConnectionType>(), It.IsAny<string>(), It.IsAny<ClientModel.DateTimeFormat>()), Times.Never());
            _httpClientFactoryMock.Verify(hcf => hcf.CreateClient("forex-miner-thor"), Times.Never());
            _instrumentStorageServiceMock.Verify(iss => iss.StoreInstrumentCandles(It.IsAny<InstrumentWithCandles>()), Times.Never());
        }

        [Fact]
        public async void CheckInstrumentGranularitiesAndLoadData_WithNewGranularities()
        {
            // Arrange
            var instrument = new Database.Instrument()
            {
                Name = ForexMiner.Heimdallr.Common.Data.Contracts.Instrument.InstrumentName.EUR_USD
            };
            var granularities = new List<Database.InstrumentGranularity>()
            {
                new Database.InstrumentGranularity()
                {
                    Id = Guid.NewGuid(),
                    Granularity = Granularity.H1,
                    State = Database.GranularityState.New,
                    Instrument = instrument
                }
            };
            instrument.Granularities = granularities;
            _dbContext.Add(instrument);
            _dbContext.SaveChanges();

            var oandaApiMock = new Mock<IOandaApiConnection>();
            var oandaInstrumentMock = new Mock<IInstrument>();
            oandaInstrumentMock.Setup(oi => oi.GetCandlesByTimeAsync(It.IsAny<ClientModel.CandlestickGranularity>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<PricingComponent>>())).Returns(
                Task.FromResult(GenerateCandles())
            );
            oandaApiMock.Setup(oa => oa.GetInstrument(It.IsAny<GeriRemenyi.Oanda.V20.Client.Model.InstrumentName>())).Returns(oandaInstrumentMock.Object);
            _oandaApiConnectionFactoryMock.Setup(oacf => oacf.CreateConnection(OandaConnectionType.FxPractice, "FakeToken", It.IsAny<ClientModel.DateTimeFormat>())).Returns(oandaApiMock.Object);

            _httpClientFactoryMock.Setup(hcf => hcf.CreateClient("forex-miner-thor")).Returns(new Mock<HttpClient>().Object);
            _instrumentStorageServiceMock.Setup(iss => iss.StoreInstrumentCandles(It.IsAny<InstrumentWithCandles>()));

            // Act
            await _instrumentHistoryService.CheckInstrumentGranularitiesAndLoadData();

            // Assert
            _oandaApiConnectionFactoryMock.Verify(oacf => oacf.CreateConnection(It.IsAny<OandaConnectionType>(), It.IsAny<string>(), It.IsAny<ClientModel.DateTimeFormat>()), Times.AtLeastOnce());
            _httpClientFactoryMock.Verify(hcf => hcf.CreateClient("forex-miner-thor"), Times.AtLeastOnce());
            _instrumentStorageServiceMock.Verify(iss => iss.StoreInstrumentCandles(It.IsAny<InstrumentWithCandles>()), Times.AtLeastOnce());
        }

        private IEnumerable<ClientModel.Candlestick> GenerateCandles()
        {
            return new List<ClientModel.Candlestick>()
            {
                new ClientModel.Candlestick()
                {
                    Time = "2020-10-10T20:00Z",
                    Volume = 10,
                    Complete = true,
                    Ask = new ClientModel.CandlestickData()
                    { 
                        O = 1.1234,
                        H = 1.1234,
                        L = 1.1234,
                        C = 1.1234
                    },
                    Mid = new ClientModel.CandlestickData()
                    {
                        O = 1.1234,
                        H = 1.1234,
                        L = 1.1234,
                        C = 1.1234
                    },
                    Bid = new ClientModel.CandlestickData()
                    {
                        O = 1.1234,
                        H = 1.1234,
                        L = 1.1234,
                        C = 1.1234
                    }
                }
            };
        }

        public void Dispose()
        {
            _dbContext.Database.CloseConnection();
        }
    }
}
