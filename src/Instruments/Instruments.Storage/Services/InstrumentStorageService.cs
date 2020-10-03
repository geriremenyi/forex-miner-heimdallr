namespace ForexMiner.Heimdallr.Instruments.Storage.Services
{
    using Azure.Storage.Blobs;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Candlestick = Model.Candlestick;

    class InstrumentStorageService : IInstrumentStorageService
    {
        public readonly BlobServiceClient _storageService;

        public InstrumentStorageService(BlobServiceClient storageService)
        {
            _storageService = storageService;
        }

        public async Task<IEnumerable<Candlestick>> GetMonthlyData(InstrumentName instrument, CandlestickGranularity granularity, int year, int month)
        {
            // Get the container client
            var containerClient = _storageService.GetBlobContainerClient(GetContainerName(instrument));

            // Get the blob client
            var blobClient = containerClient.GetBlobClient(GetBlobName(granularity, year, month));

            // Download blob to stream
            using var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);

            // Deserialize stream to object
            stream.Position = 0;
            var serializer = new JsonSerializer();
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var candles = serializer.Deserialize<IEnumerable<Candlestick>>(jsonTextReader);

            return candles;
        }

        public async Task AddMonthlyData(InstrumentName instrument, CandlestickGranularity granularity, int year, int month, IEnumerable<Candlestick> candles)
        {
            // Get or create container
            var containerClient = _storageService.GetBlobContainerClient(GetContainerName(instrument));
            await containerClient.CreateIfNotExistsAsync();

            // Serialize object to memory stream
            var serializedCandles = JsonConvert.SerializeObject(candles);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedCandles));

            // Upload to blob
            await containerClient.UploadBlobAsync(GetBlobName(granularity, year, month), stream);
        }

        private string GetContainerName(InstrumentName instrument) => instrument.ToString().Replace("_", "").ToLower();

        private string GetBlobName(CandlestickGranularity granularity, int year, int month) => $"{granularity.ToString().ToUpper()}/{year}_{month}.json";
    }
}
