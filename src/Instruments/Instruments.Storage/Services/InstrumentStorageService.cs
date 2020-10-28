//----------------------------------------------------------------------------------------
// <copyright file="InstrumentStorageService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Storage.Services
{
    using Azure.Storage.Blobs;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Instruments.Storage.Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Storage account based instrument store implementation
    /// </summary>
    class InstrumentStorageService : IInstrumentStorageService
    {
        /// <summary>
        /// Name of the container the instruments are held in
        /// </summary>
        private const string INSTRUMENT_CONTAINER = "instruments";

        /// <summary>
        /// The actual storage account service client
        /// </summary>
        private readonly BlobServiceClient _storageService;

        /// <summary>
        /// Storage account based instrument storage constructor
        /// Sets up the required services
        /// </summary>
        /// <param name="storageService">The blob client service</param>
        public InstrumentStorageService(BlobServiceClient storageService)
        {
            _storageService = storageService;
        }

        /// <summary>
        /// Get instrument with it's candles for a specific timerange
        /// with a specific granularity
        /// </summary>
        /// <param name="instrument">Name of the instrument</param>
        /// <param name="granularity">Granularity of the instrument</param>
        /// <param name="utcFrom">Time to get the candles from (taken as an UTC time)</param>
        /// <param name="utcTo">Time to get the candles to (taken as an UTC time)</param>
        /// <returns>The instrument object with it's candles</returns>
        public async Task<InstrumentWithCandles> GetInstrumentCandles(InstrumentName instrument, Granularity granularity, DateTime utcFrom, DateTime utcTo)
        {
            // Collect all blob names needed
            var blobNames = GetBlobPaths(instrument, granularity, utcFrom, utcTo);

            // Get the container client
            var containerClient = _storageService.GetBlobContainerClient(INSTRUMENT_CONTAINER);

            // Create returning instrument object
            var instrumentWithCandles = new InstrumentWithCandles()
            { 
                InstrumentName = instrument,
                Granularity = granularity,
                Candles = new List<Candle>()
            };

            // Loop through the required blob files to get all candles data
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
                // Since it is abstract ICollection
                // Do a loop
                foreach (var candle in candles)
                {
                    instrumentWithCandles.Candles.Add(candle);
                }
            }

            // Order candles by time (better safe then sorry)
            instrumentWithCandles.Candles.OrderBy(candle => candle.Time);

            // Return instrument with it's candles
            return instrumentWithCandles;
        }

        /// <summary>
        /// Store candles
        /// </summary>
        /// <param name="instrument">The instrument object to store candles for</param>
        /// <returns></returns>
        public async Task StoreInstrumentCandles(InstrumentWithCandles instrument)
        {
            // Get or create container
            var containerClient = _storageService.GetBlobContainerClient(INSTRUMENT_CONTAINER);
            await containerClient.CreateIfNotExistsAsync();

            // Explode instrument to monthly candles list
            var candlesMonthly = instrument.Candles.ToLookup(c => new { c.Time.ToUniversalTime().Year, c.Time.ToUniversalTime().Month });

            // Base folder name
            var instrumentGranularityFolder = GetInstrumentFolder(instrument.InstrumentName, instrument.Granularity);

            // Upload that monthly data
            foreach (var yearAndMonth in candlesMonthly)
            {
                // Serialize object to memory stream
                var serializedCandles = JsonConvert.SerializeObject(candlesMonthly[yearAndMonth.Key]);
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedCandles));

                // Upload to blob
                await containerClient.GetBlobClient($"{instrumentGranularityFolder}/{GetMonthlyFile(yearAndMonth.Key.Year, yearAndMonth.Key.Month)}").UploadAsync(stream, true);
            }
        }

        /// <summary>
        /// Get all the blob paths which contains at least the data
        /// for the period of time between from and two given
        /// </summary>
        /// <param name="instrument"></param>
        /// <param name="granularity"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private IEnumerable<string> GetBlobPaths(InstrumentName instrument, Granularity granularity, DateTime from, DateTime to)
        {
            // Check parameters
            CheckFromAndTo(from, to);

            // Base folder name
            var instrumentGranularityFolder = GetInstrumentFolder(instrument, granularity);

            // Collect all blob files needed
            var blobs = new List<string>();
            var candleCurrentTime = from.GetInclusiveCandleTime(granularity).ToUniversalTime();
            var candleEndTime = to.GetInclusiveCandleTime(granularity).ToUniversalTime();
            while ((((candleEndTime.Year - candleCurrentTime.Year) * 12) + candleEndTime.Month - candleCurrentTime.Month) >= 0)
            {
                blobs.Add($"{instrumentGranularityFolder}/{GetMonthlyFile(candleCurrentTime.Year, candleCurrentTime.Month)}");
                candleCurrentTime = candleCurrentTime.AddMonths(1);
            }

            return blobs;
        }

        /// <summary>
        /// Check that from and to dates are reasonable
        /// </summary>
        /// <param name="from">Requested from date</param>
        /// <param name="to">Requested to date</param>
        private void CheckFromAndTo(DateTimeOffset from, DateTimeOffset to)
        {
            // From is not in future
            if (from > DateTimeOffset.Now)
            {
                throw new ArgumentException("The 'from' time cannot be in the future.");
            }

            // To is not in future
            if (to > DateTimeOffset.Now)
            {
                throw new ArgumentException("The 'to' time cannot be in the future.");
            }

            // From is not bigger than to
            if (from > to)
            {
                throw new ArgumentException("The 'from' time cannot be later than the 'to' time.");
            }
        }

        /// <summary>
        /// Get the instrument and granularity based blob folder
        /// </summary>
        /// <param name="instrument"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        private string GetInstrumentFolder(InstrumentName instrument, Granularity granularity) => $"{instrument.ToString().ToUpper()}/{granularity.ToString().ToUpper()}";

        /// <summary>
        /// Get the monthly data file based on the year and month
        /// </summary>
        /// <param name="year">Year of the data</param>
        /// <param name="month">Month of the data</param>
        /// <returns></returns>
        private string GetMonthlyFile(int year, int month) => $"{year:D4}/{month:D2}.json";
    }
}
