namespace Instruments.Api.Tests.Controllers.V1
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Instruments.Api.Controllers.V1;
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class InstrumentCandlesControllerTests
    {
        private readonly Mock<IInstrumentStorageService> _instrumentStorageServiceMock;
        private readonly InstrumentCandlesController _instrumentCandlesController;

        public InstrumentCandlesControllerTests()
        {
            _instrumentStorageServiceMock = new Mock<IInstrumentStorageService>();
            _instrumentCandlesController = new InstrumentCandlesController(_instrumentStorageServiceMock.Object);
        }

        [Fact]
        public async void GetInstrumentCandles()
        {
            // Arrange
            var instrument = InstrumentName.EUR_USD;
            var granularity = Granularity.H1;
            var from = DateTime.UtcNow.AddDays(-5);
            var to = DateTime.UtcNow.AddDays(-5).AddHours(1);
            var mockCandles = new InstrumentWithCandles()
            {
                InstrumentName = instrument,
                Granularity = granularity,
                Candles = new List<Candle>()
                {
                    new Candle()
                {
                    Time = DateTime.UtcNow.AddDays(-5),
                    Volume = 1234,
                    Bid = new Candlestick()
                    {
                        Open = 1.1234,
                        High = 1.1234,
                        Low = 1.1234,
                        Close = 1.1234
                    },
                    Mid = new Candlestick()
                    {
                        Open = 1.1234,
                        High = 1.1234,
                        Low = 1.1234,
                        Close = 1.1234
                    },
                    Ask = new Candlestick()
                    {
                        Open = 1.1234,
                        High = 1.1234,
                        Low = 1.1234,
                        Close = 1.1234
                    },
                },
                new Candle()
                {
                    Time = DateTime.UtcNow.AddDays(1),
                    Volume = 1234,
                    Bid = new Candlestick()
                    {
                        Open = 1.1234,
                        High = 1.1234,
                        Low = 1.1234,
                        Close = 1.1234
                    },
                    Mid = new Candlestick()
                    {
                        Open = 1.1234,
                        High = 1.1234,
                        Low = 1.1234,
                        Close = 1.1234
                    },
                    Ask = new Candlestick()
                    {
                        Open = 1.1234,
                        High = 1.1234,
                        Low = 1.1234,
                        Close = 1.1234
                    },
                }
                }
            };
            _instrumentStorageServiceMock.Setup(iss => iss.GetInstrumentCandles(instrument, granularity, from, to)).Returns(Task.FromResult(mockCandles));

            // Act
            var instrumentWithCandles = await _instrumentCandlesController.GetInstrumentCandles(instrument, granularity, from, to);

            // Assert
            _instrumentStorageServiceMock.Verify(iss => iss.GetInstrumentCandles(instrument, granularity, from, to), Times.Once());
            Assert.Equal(mockCandles, instrumentWithCandles);
        }
    }
}
