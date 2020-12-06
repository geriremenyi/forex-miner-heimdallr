namespace Instruments.Api.Tests.Controllers.V1
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Instruments.Api.Controllers.V1;
    using ForexMiner.Heimdallr.Instruments.Api.Services;
    using Moq;
    using System.Collections.Generic;
    using Xunit;

    public class InstrumentsControllerTests
    {
        private readonly Mock<IInstrumentService> _instrumentServiceMock;
        private readonly InstrumentsController _instrumentsController;

        public InstrumentsControllerTests()
        {
            _instrumentServiceMock = new Mock<IInstrumentService>();
            _instrumentsController = new InstrumentsController(_instrumentServiceMock.Object);
        }

        [Fact]
        public void GetAllInstruments()
        {
            // Arrange
            var instruments = new List<Instrument>()
            {
                new Instrument()
                {
                    Name = InstrumentName.EUR_USD,
                    Granularities = new List<InstrumentGranularity>()
                    { 
                        new InstrumentGranularity()
                        { 
                            Granularity = Granularity.H1,
                            IsTradeable = true
                        },
                        new InstrumentGranularity()
                        {
                            Granularity = Granularity.M1,
                            IsTradeable = false
                        }
                    }
                },
                new Instrument()
                {
                    Name = InstrumentName.AUD_USD,
                    Granularities = new List<InstrumentGranularity>()
                    {
                        new InstrumentGranularity()
                        {
                            Granularity = Granularity.M,
                            IsTradeable = false
                        },
                    }
                }
            };
            _instrumentServiceMock.Setup(ism => ism.GetAllInstruments()).Returns(instruments);

            // Act
            var allInstruments = _instrumentsController.GetAllInstrument();

            // Assert
            _instrumentServiceMock.Verify(ism => ism.GetAllInstruments(), Times.Once());
            Assert.Equal(instruments, allInstruments);
        }

        [Fact]
        public void AddInstrument()
        {
            // Arrange
            var instrumentCreation = new InstrumentCreation()
            {
                Name = InstrumentName.EUR_USD
            };
            var createdInstrument = new Instrument()
            {
                Name = instrumentCreation.Name
            };
            _instrumentServiceMock.Setup(ism => ism.AddInstrument(instrumentCreation)).Returns(createdInstrument);

            // Act
            var instrumentCreated = _instrumentsController.AddInstrument(instrumentCreation);

            // Assert
            _instrumentServiceMock.Verify(ism => ism.AddInstrument(instrumentCreation), Times.Once());
            Assert.Equal(createdInstrument, instrumentCreated);
        }

        [Fact]
        public void AddGranularity()
        {
            // Arrange
            var granularityCreation = new InstrumentGranularityCreation()
            {
                Granularity = Granularity.M1
            };
            var instrument = new Instrument()
            {
                Name = InstrumentName.EUR_USD,
                Granularities = new List<InstrumentGranularity>()
                { 
                    new InstrumentGranularity()
                    { 
                        Granularity = granularityCreation.Granularity,
                        IsTradeable = false
                    }
                }
            };
            _instrumentServiceMock.Setup(ism => ism.AddGranularity(instrument.Name, granularityCreation)).Returns(instrument);

            // Act
            var instrumentExtended = _instrumentsController.AddGranularity(instrument.Name, granularityCreation);

            // Assert
            _instrumentServiceMock.Verify(ism => ism.AddGranularity(instrument.Name, granularityCreation), Times.Once());
            Assert.Equal(instrumentExtended, instrument);
        }
    }
}
