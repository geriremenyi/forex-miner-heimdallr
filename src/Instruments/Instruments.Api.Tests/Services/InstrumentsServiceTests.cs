namespace Instruments.Api.Tests.Services
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using ForexMiner.Heimdallr.Common.Data.Exceptions;
    using ForexMiner.Heimdallr.Common.Data.Mapping;
    using ForexMiner.Heimdallr.Instruments.Api.Services;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Xunit;
    using Contracts = ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using Database = ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument;

    public class InstrumentsServiceTests: IDisposable
    {

        private readonly ForexMinerHeimdallrDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IInstrumentService _instrumentService;

        public InstrumentsServiceTests()
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

            // Class under test
            _instrumentService = new InstrumentService(_dbContext, _mapper);
        }

        [Fact]
        public void GetAllInstruments()
        {
            // Arrange
            var instrument1 = new Database.Instrument()
            {
                Name = Contracts.InstrumentName.EUR_USD
            };
            var granularity1 = new Database.InstrumentGranularity()
            {
                Id = Guid.NewGuid(),
                Instrument = instrument1,
                Granularity = Contracts.Granularity.H1,
                State = Database.GranularityState.New
            };
            instrument1.Granularities.Add(granularity1);
            _dbContext.Add(instrument1);

            var instrument2 = new Database.Instrument()
            {
                Name = Contracts.InstrumentName.NZD_USD
            };
            var granularity2 = new Database.InstrumentGranularity()
            {
                Id = Guid.NewGuid(),
                Instrument = instrument2,
                Granularity = Contracts.Granularity.M1,
                State = Database.GranularityState.Tradeable
            };
            instrument2.Granularities.Add(granularity2);
            _dbContext.Add(instrument2);

            _dbContext.SaveChanges();

            // Act
            var instrumentGranularities = _instrumentService.GetAllInstruments();

            // Assert
            instrumentGranularities = instrumentGranularities.OrderBy(instrument => instrument.Name);
            Assert.Equal(2, instrumentGranularities.Count());
            Assert.Equal(instrument1.Name, instrumentGranularities.ElementAt(0).Name);
            Assert.Single(instrumentGranularities.ElementAt(0).Granularities);
            Assert.Equal(granularity1.Granularity, instrumentGranularities.ElementAt(0).Granularities.ElementAt(0).Granularity);
            Assert.Equal(instrument2.Name, instrumentGranularities.ElementAt(1).Name);
            Assert.Single(instrumentGranularities.ElementAt(1).Granularities);
            Assert.Equal(granularity2.Granularity, instrumentGranularities.ElementAt(1).Granularities.ElementAt(0).Granularity);
        }

        [Fact]
        public void AddInstrument()
        {
            // Arrange
            var instrumentName = Contracts.InstrumentName.EUR_USD;
            var instrument = new Contracts.InstrumentCreation()
            {
                Name = instrumentName
            };

            // Act
            var instrumentAdded = _instrumentService.AddInstrument(instrument);

            // Assert
            Assert.Equal(instrumentName, instrumentAdded.Name);
        }

        [Fact]
        public void AddInstrument_Existing()
        {
            // Arrange
            var instrumentName = Contracts.InstrumentName.EUR_USD;
            var instrument1 = new Database.Instrument()
            {
                Name = instrumentName
            };
            var granularity1 = new Database.InstrumentGranularity()
            {
                Id = Guid.NewGuid(),
                Instrument = instrument1,
                Granularity = Contracts.Granularity.H1,
                State = Database.GranularityState.New
            };
            instrument1.Granularities.Add(granularity1);
            _dbContext.Add(instrument1);
            _dbContext.SaveChanges();

            var instrument = new Contracts.InstrumentCreation()
            {
                Name = instrumentName
            };

            // Act
            var instrumentAdded = _instrumentService.AddInstrument(instrument);

            // Assert
            Assert.Equal(instrumentName, instrumentAdded.Name);
        }

        [Fact]
        public void AddGranularity()
        {
            // Arrange
            var instrumentName = Contracts.InstrumentName.EUR_USD;
            var instrument = new Database.Instrument()
            {
                Name = instrumentName
            };
            _dbContext.Add(instrument);
            _dbContext.SaveChanges();

            var granularity = new Contracts.InstrumentGranularityCreation()
            {
                Granularity = Contracts.Granularity.H1,
            };

            // Act
            var instrumentGranularity = _instrumentService.AddGranularity(instrumentName, granularity);

            // Assert
            Assert.Equal(instrumentName, instrumentGranularity.Name);
            Assert.Single(instrumentGranularity.Granularities);
            Assert.Equal(granularity.Granularity, instrumentGranularity.Granularities.ElementAt(0).Granularity);
        }

        [Fact]
        public void AddGranularity_MissingInstrument()
        {
            // Arrange
            var instrumentName = Contracts.InstrumentName.EUR_USD;

            var granularity = new Contracts.InstrumentGranularityCreation()
            {
                Granularity = Contracts.Granularity.H1,
            };

            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _instrumentService.AddGranularity(instrumentName, granularity));
            Assert.Equal(HttpStatusCode.NotFound, exception.Status);
        }

        [Fact]
        public void AddGranularity_Existing()
        {
            // Arrange
            var instrumentName = Contracts.InstrumentName.EUR_USD;
            var instrument = new Database.Instrument()
            {
                Name = instrumentName
            };
            var granularityName = Contracts.Granularity.H1;
            var granularity = new Database.InstrumentGranularity()
            {
                Id = Guid.NewGuid(),
                Instrument = instrument,
                Granularity = granularityName,
                State = Database.GranularityState.New
            };
            _dbContext.Add(instrument);
            _dbContext.SaveChanges();

            var granularityToAdd = new Contracts.InstrumentGranularityCreation()
            {
                Granularity = granularityName
            };

            // Act
            var instrumentGranularity = _instrumentService.AddGranularity(instrumentName, granularityToAdd);

            // Assert
            Assert.Equal(instrumentName, instrumentGranularity.Name);
            Assert.Single(instrumentGranularity.Granularities);
            Assert.Equal(granularity.Granularity, instrumentGranularity.Granularities.ElementAt(0).Granularity);
        }

        public void Dispose()
        {
            _dbContext.Database.CloseConnection();
        }
    }
}
