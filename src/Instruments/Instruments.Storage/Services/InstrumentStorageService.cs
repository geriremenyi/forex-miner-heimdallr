namespace ForexMiner.Heimdallr.Instruments.Storage.Services
{
    using Azure.Storage.Blobs;
    using ForexMiner.Heimdallr.Instruments.Storage.Model;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Instrument = Model.Instrument;

    class InstrumentStorageService : IInstrumentStorageService
    {
        private static readonly string INSTRUMENT_CONTAINER = "instruments";

        private readonly BlobServiceClient _storageService;

        public InstrumentStorageService(BlobServiceClient storageService)
        {
            _storageService = storageService;
        }

        public async Task<Instrument> GetInstrumentCandles(InstrumentName instrument, Granularity granularity, DateTime from, DateTime to)
        {
            // Collect all blob names needed
            var blobNames = GetBlobNames(instrument, granularity, from, to);

            // Get the container client
            var containerClient = _storageService.GetBlobContainerClient(INSTRUMENT_CONTAINER);

            // Merge all data into one instrument object
            var instrumentWithCandles = new Instrument(instrument, granularity, from.GetInclusiveCandleTime(granularity), to.GetInclusiveCandleTime(granularity));
            foreach (string blob in blobNames)
            {
                // Get the blob client
                var blobClient = containerClient.GetBlobClient(blob);

                // Download blob to stream
                using var stream = new MemoryStream();
                await blobClient.DownloadToAsync(stream);

                // Deserialize stream to object
                stream.Position = 0;
                var serializer = new JsonSerializer();
                using var streamReader = new StreamReader(stream);
                using var jsonTextReader = new JsonTextReader(streamReader);
                var candles = serializer.Deserialize<IEnumerable<Candle>>(jsonTextReader);

                // Add it to the instrument
                instrumentWithCandles.Candles.AddRange(candles);
            }

            return instrumentWithCandles;
        }

        public async Task StoreInstrumentCandles(Instrument instrument)
        {
            // Get or create container
            var containerClient = _storageService.GetBlobContainerClient(INSTRUMENT_CONTAINER);
            await containerClient.CreateIfNotExistsAsync();

            // Explode instrument to monthly candles list
            var candlesMonthly = instrument.Candles.ToLookup(c => new { c.Time.Year, c.Time.Month });

            // Base folder name
            var instrumentGranularityFolder = GetInstrumentFolder(instrument.Name, instrument.Granularity);

            // Upload that monthly data
            foreach (var yearAndMonth in candlesMonthly)
            {
                // Serialize object to memory stream
                var serializedCandles = JsonConvert.SerializeObject(candlesMonthly[yearAndMonth.Key]);
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedCandles));

                // Upload to blob
                await containerClient.GetBlobClient($"{instrumentGranularityFolder}/{GetTimeFile(yearAndMonth.Key.Year, yearAndMonth.Key.Month)}").UploadAsync(stream, true);
            }
        }

        private IEnumerable<string> GetBlobNames(InstrumentName instrument, Granularity granularity, DateTime from, DateTime to)
        {
            // Check parameters
            CheckFromAndTo(from, to);

            // Base folder name
            var instrumentGranularityFolder = GetInstrumentFolder(instrument, granularity);

            // Collect all blob files needed
            var blobs = new List<string>();
            var candleCurrentTime = from.GetInclusiveCandleTime(granularity);
            var candleEndTime = to.GetInclusiveCandleTime(granularity);
            while ((((candleEndTime.Year - candleCurrentTime.Year) * 12) + candleEndTime.Month - candleCurrentTime.Month) >= 0)
            {
                blobs.Add($"{instrumentGranularityFolder}/{GetTimeFile(candleCurrentTime.Year, candleCurrentTime.Month)}");
                candleCurrentTime = candleCurrentTime.AddMonths(1);
            }

            return blobs;
        }

        private void CheckFromAndTo(DateTime from, DateTime to)
        {
            // From is not in future
            if (from > DateTime.Now)
            {
                throw new ArgumentException("The 'from' time cannot be in the future.");
            }

            // To is not in future
            if (to > DateTime.Now)
            {
                throw new ArgumentException("The 'to' time cannot be in the future.");
            }

            // From is not bigger than to
            if (from > to)
            {
                throw new ArgumentException("The 'from' time cannot be later than the 'to' time.");
            }
        }

        private string GetInstrumentFolder(InstrumentName instrument, Granularity granularity) => $"{instrument.ToString().ToUpper()}/{granularity.ToString().ToUpper()}";
        private string GetTimeFile(int year, int month) => $"{year:D4}/{month:D2}.json";
    }
}
