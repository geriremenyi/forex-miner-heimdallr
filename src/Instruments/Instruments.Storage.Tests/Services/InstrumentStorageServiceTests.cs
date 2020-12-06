namespace Instruments.Storage.Tests.Services
{
    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using Moq;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using Xunit;

    public class InstrumentStorageServiceTests
    {
        private readonly Mock<BlobServiceClient> _storageClientMock;
        private readonly IInstrumentStorageService _instrumentStorageService;

        public InstrumentStorageServiceTests()
        {
            _storageClientMock = new Mock<BlobServiceClient>();
            _instrumentStorageService = new InstrumentStorageService(_storageClientMock.Object);
        }

        [Fact]
        public async void GetInstrumentCandles()
        {
            // Arrange
            var instrument = InstrumentName.EUR_USD;
            var granularity = Granularity.H1;
            var now = DateTime.UtcNow;
            var from = new DateTime(now.Year - 2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(now.Year - 2, 12, 31, 23, 59, 0, DateTimeKind.Utc);
            var blobClientMock = new Mock<BlobClient>();
            blobClientMock.Setup(bc => bc.DownloadToAsync(It.IsAny<Stream>())).Callback<Stream>(candleStream => WriteSerializedCandlesToStream(candleStream));
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            blobContainerClientMock.Setup(bcc => bcc.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            _storageClientMock.Setup(sc => sc.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClientMock.Object);

            // Act
            var candles = await _instrumentStorageService.GetInstrumentCandles(instrument, granularity, from, to);

            // Assert
            _storageClientMock.Verify(sc => sc.GetBlobContainerClient(It.IsAny<string>()), Times.Once());
            blobContainerClientMock.Verify(bcc => bcc.GetBlobClient(It.IsAny<string>()), Times.Exactly(12));
            blobClientMock.Verify(bc => bc.DownloadToAsync(It.IsAny<Stream>()), Times.Exactly(12));
        }

        [Fact]
        public async void GetInstrumentCandles_WrongFromIsBiggerThanTo()
        {
            // Arrange
            var instrument = InstrumentName.EUR_USD;
            var granularity = Granularity.H1;
            var from = DateTime.UtcNow.AddYears(-10);
            var to = DateTime.UtcNow.AddYears(-15);

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _instrumentStorageService.GetInstrumentCandles(instrument, granularity, from, to));
            Assert.Contains("cannot be later than", exception.Message);
        }

        [Fact]
        public async void GetInstrumentCandles_FromIsLaterThanNow()
        {
            // Arrange
            var instrument = InstrumentName.EUR_USD;
            var granularity = Granularity.H1;
            var from = DateTime.UtcNow.AddYears(10);
            var to = DateTime.UtcNow.AddYears(15);

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _instrumentStorageService.GetInstrumentCandles(instrument, granularity, from, to));
            Assert.Contains("'from' time cannot be in the future", exception.Message);
        }

        [Fact]
        public async void GetInstrumentCandles_ToIsLaterThanNow()
        {
            // Arrange
            var instrument = InstrumentName.EUR_USD;
            var granularity = Granularity.H1;
            var from = DateTime.UtcNow.AddYears(-1);
            var to = DateTime.UtcNow.AddYears(3);

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _instrumentStorageService.GetInstrumentCandles(instrument, granularity, from, to));
            Assert.Contains("'to' time cannot be in the future", exception.Message);
        }

        [Fact]
        public async void StoreInstrumentCandles_WithoutExistingData()
        {
            // Arrange
            var instrument = InstrumentName.EUR_USD;
            var granularity = Granularity.H1;
            var candles = new List<Candle>()
            {
                new Candle()
                {
                    Time = new DateTime(2015, 5, 12, 10, 00, 00, DateTimeKind.Utc),
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
                    Time = new DateTime(2015, 6, 12, 10, 00, 00, DateTimeKind.Utc),
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
            };
            var instrumentWithCandles = new InstrumentWithCandles()
            {
                InstrumentName = instrument,
                Granularity = granularity,
                Candles = candles
            };
            var blobClientMock = new Mock<BlobClient>();
            blobClientMock.Setup(bc => bc.UploadAsync(It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()));
            blobClientMock.Setup(bc => bc.DownloadToAsync(It.IsAny<Stream>())).Throws(new Exception()); // No already existing data found for the given months
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            blobContainerClientMock.Setup(bcc => bcc.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), null, null, It.IsAny<CancellationToken>()));
            blobContainerClientMock.Setup(bcc => bcc.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            _storageClientMock.Setup(sc => sc.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClientMock.Object);

            // Act
            await _instrumentStorageService.StoreInstrumentCandles(instrumentWithCandles);

            // Assert
            blobClientMock.Verify(bc => bc.DownloadToAsync(It.IsAny<Stream>()), Times.Exactly(2));
            blobClientMock.Verify(bc => bc.UploadAsync(It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async void StoreInstrumentCandles_WithExistingData()
        {
            // Arrange
            var instrument = InstrumentName.EUR_USD;
            var granularity = Granularity.H1;
            var candles = new List<Candle>()
            {
                new Candle()
                {
                    Time = DateTime.UtcNow.AddDays(-3).AddHours(1),
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
            };
            var instrumentWithCandles = new InstrumentWithCandles()
            {
                InstrumentName = instrument,
                Granularity = granularity,
                Candles = candles
            };
            var blobClientMock = new Mock<BlobClient>();
            blobClientMock.Setup(bc => bc.UploadAsync(It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()));
            blobClientMock.Setup(bc => bc.DownloadToAsync(It.IsAny<Stream>())).Callback<Stream>(candleStream => WriteSerializedCandlesToStream(candleStream)); // There is already existing data for the month
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            blobContainerClientMock.Setup(bcc => bcc.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), null, null, It.IsAny<CancellationToken>()));
            blobContainerClientMock.Setup(bcc => bcc.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            _storageClientMock.Setup(sc => sc.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClientMock.Object);

            // Act
            await _instrumentStorageService.StoreInstrumentCandles(instrumentWithCandles);

            // Assert
            blobClientMock.Verify(bc => bc.UploadAsync(It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        private void WriteSerializedCandlesToStream(Stream toStream)
        {
            var candles = new List<Candle>()
            {
                new Candle()
                {
                    Time = DateTime.UtcNow.AddDays(-3),
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
            };

            var serializedObject = JsonConvert.SerializeObject(candles);
            toStream.Write(Encoding.UTF8.GetBytes(serializedObject));
        }
    }
}
