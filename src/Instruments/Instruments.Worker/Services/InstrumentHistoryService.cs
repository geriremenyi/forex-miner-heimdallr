//----------------------------------------------------------------------------------------
// <copyright file="HistoryService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Worker.Services
{
    
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Polly;
    using Polly.Retry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.IO;
    using System.Text;
    using System.Net.Http.Headers;
    using AutoMapper;
    using GeriRemenyi.Oanda.V20.Sdk;
    using GeriRemenyi.Oanda.V20.Sdk.Utilities;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using Contracts = Heimdallr.Common.Data.Contracts.Instrument;
    using Database = Heimdallr.Common.Data.Database.Models.Instrument;
    using GeriRemenyi.Oanda.V20.Client.Model;

    /// <summary>
    /// History service implementation
    /// </summary>
    class InstrumentHistoryService : IInstrumentHistoryService
    {
        /// <summary>
        /// Collect instrument history data starting this date
        /// </summary>
        private static readonly DateTime START_TIME = new DateTime(2010, 1, 1);

        /// <summary>
        /// Configuration object
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Http client factory to push to engine API
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Polly retry policy for the engine requests
        /// </summary>
        private readonly AsyncRetryPolicy _retryPolicy;

        /// <summary>
        /// Database context
        /// </summary>
        private readonly ForexMinerHeimdallrDbContext _dbContext;

        /// <summary>
        /// Instrument storage service
        /// </summary>
        private readonly IInstrumentStorageService _instrumentStorageService;

        /// <summary>
        /// Auto mapper service
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Instruments history service constructor
        /// Sets up the required services and objects
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        /// <param name="httpClientFactory">Http client factory</param>
        /// <param name="dbContext">Database context</param>
        /// <param name="instrumentStorageService">Instrument storage service</param>
        /// <param name="mapper">Auto mapper service</param>
        public InstrumentHistoryService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ForexMinerHeimdallrDbContext dbContext,
            IInstrumentStorageService instrumentStorageService,
            IMapper mapper
        )
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(int.Parse(_configuration["forex-miner-thor:Max-Retries"]));
            _dbContext = dbContext;
            _instrumentStorageService = instrumentStorageService;
            _mapper = mapper;
        }

        /// <summary>
        /// Check if there are any new instruments granularities present in the DB
        /// and if yes then load it's historical data
        /// </summary>
        /// <returns>A async task</returns>
        public async Task CheckInstrumentGranularitiesAndLoadData()
        {
            // Collect all new granularities
            var newGranularities = GetNewGranularities();

            // Mark them as in process
            foreach (var granularity in newGranularities)
            {
                granularity.State = Database.GranularityState.InDataLoading;
            }
            if (newGranularities.Count() > 0)
            {
                // Commit changes in the DB
                _dbContext.SaveChanges();

                // Get the data loading ones
                var dataLoadingGranularities = GetInDatLoadingGranularities();

                // Actually start processing them
                foreach (var granularity in dataLoadingGranularities)
                {
                    // Monthly processing
                    var utcBeginningOfTheMonth = new DateTime(START_TIME.Year, START_TIME.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    var utcNow = DateTime.UtcNow;
                    var utcEndTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.AddDays(-1).Day, 23, 59, 59);
                    while (utcBeginningOfTheMonth <= utcEndTime)
                    {
                        var endOfTheMonth = new DateTime(utcBeginningOfTheMonth.Year, utcBeginningOfTheMonth.Month, utcBeginningOfTheMonth.AddMonths(1).AddDays(-1).Day, 0, 0, 0, DateTimeKind.Utc);
                        if (endOfTheMonth > utcEndTime)
                        {
                            endOfTheMonth = utcEndTime;
                        }

                        var candlesOfAMonth = await GetCandlesBetween(granularity.Instrument.Name, granularity.Granularity, utcBeginningOfTheMonth, endOfTheMonth);

                        // Push it to the engine
                        await PostCandlesToTheEngine(granularity.Instrument.Name, granularity.Granularity, candlesOfAMonth);

                        // Push it to the storage account
                        await _instrumentStorageService.StoreInstrumentCandles(new Contracts.InstrumentWithCandles()
                        {
                            InstrumentName = granularity.Instrument.Name,
                            Granularity = granularity.Granularity,
                            Candles = candlesOfAMonth
                        });

                        utcBeginningOfTheMonth = utcBeginningOfTheMonth.AddMonths(1);
                    }

                    // Update granularity state to data loaded
                    granularity.State = Database.GranularityState.Tradeable;
                }

                // Save changes to DB
                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get the candles in monthly packages
        /// </summary>
        /// <returns>Candles in a month</returns>
        private async Task<ICollection<Contracts.Candle>> GetCandlesBetween(Contracts.InstrumentName instrument, Contracts.Granularity granularity, DateTime utcFrom, DateTime utcTo)
        {
            // Authenticate to OANDA with the master account
            ApiConnection oandaConnection = new ApiConnection(OandaServer.FxPractice, _configuration["Oanda-MasterToken"]);

            // Get candles
            var oandaInstrument = _mapper.Map<InstrumentName>(instrument);
            var oandaGranularity = _mapper.Map<CandlestickGranularity>(granularity);
            var candles = await oandaConnection.GetInstrument(oandaInstrument).GetCandles(oandaGranularity, utcFrom, utcTo);

            return _mapper.Map<ICollection<Contracts.Candle>>(candles);
        }

        /// <summary>
        /// Get new granularities from database
        /// </summary>
        /// <returns>The granularities in 'New' state</returns>
        private IEnumerable<Database.InstrumentGranularity> GetNewGranularities()
        {
            return _dbContext.Instruments
                .Include(instrument => instrument.Granularities)
                .SelectMany(instrument => instrument.Granularities)
                .Where(granularity => granularity.State == Database.GranularityState.New)
                .Include(granularity => granularity.Instrument);
        }

        /// <summary>
        /// Get in data loading granularities from database
        /// </summary>
        /// <returns>The granularities in 'New' state</returns>
        private IEnumerable<Database.InstrumentGranularity> GetInDatLoadingGranularities()
        {
            return _dbContext.Instruments
                .Include(instrument => instrument.Granularities)
                .SelectMany(instrument => instrument.Granularities)
                .Where(granularity => granularity.State == Database.GranularityState.InDataLoading)
                .Include(granularity => granularity.Instrument);
        }

        private async Task PostCandlesToTheEngine(Contracts.InstrumentName instrument, Contracts.Granularity granularity, IEnumerable<Contracts.Candle> candles)
        {
            var client = _httpClientFactory.CreateClient(_configuration["forex-miner-thor:Name"]);
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await client.PostAsync($"instruments/{instrument}/granularities/{granularity}", CreateHttpContent(candles));
            });
        }

        /// <summary>
        /// Craete HttpContent out of candles
        /// </summary>
        /// <param name="candles">The candles to convert to HttpContent</param>
        /// <returns>THe HttpContent</returns>
        private HttpContent CreateHttpContent(IEnumerable<Contracts.Candle> candles)
        {
            HttpContent httpContent = null;
            if (candles != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(candles, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue(_configuration["forex-miner-thor:Content-Type"]);
            }

            return httpContent;
        }

        /// <summary>
        /// Serialize candles to stream
        /// </summary>
        /// <param name="candles">Candles to serialize</param>
        /// <param name="stream">The stream to serialize to</param>
        public void SerializeJsonIntoStream(IEnumerable<Contracts.Candle> candles, Stream stream)
        {
            using var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);
            using var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None };
            var js = new JsonSerializer();
            js.Serialize(jtw, candles);
            jtw.Flush();
        }
    }
}