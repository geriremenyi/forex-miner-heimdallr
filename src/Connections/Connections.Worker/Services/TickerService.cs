//----------------------------------------------------------------------------------------
// <copyright file="TickerService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Worker.Services
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Database = Heimdallr.Common.Data.Database.Models;
    using Oanda = GeriRemenyi.Oanda.V20.Client.Model;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Polly;
    using Microsoft.Extensions.Configuration;
    using Polly.Retry;
    using GeriRemenyi.Oanda.V20.Sdk;
    using GeriRemenyi.Oanda.V20.Sdk.Utilities;
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Trade;
    using System.IO;
    using System.Net.Http.Headers;
    using System.Text;
    using Newtonsoft.Json;
    using DbConnections = Heimdallr.Common.Data.Database.Models.Connection;
    using ContractConnections = Heimdallr.Common.Data.Contracts.Connection;
    using ForexMiner.Heimdallr.Connections.Secret.Services;

    /// <summary>
    /// Ticker service implementation
    /// </summary>
    public class TickerService : ITickerService
    {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly ForexMinerHeimdallrDbContext _dbContext;

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
        /// The connections secret service
        /// </summary>
        private readonly IConnectionsSecretService _connectionsSecretService;

        /// <summary>
        /// Auto mapper service
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Ticker service constructor
        /// Sets up the required service
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        /// <param name="dbContext">Database context</param>
        /// <param name="httpClientFactory">Http client factory</param>
        /// <param name="mapper">Mapper service</param>
        public TickerService(
            IConfiguration configuration,
            ForexMinerHeimdallrDbContext dbContext, 
            IHttpClientFactory httpClientFactory,
            IConnectionsSecretService connectionsSecretService,
            IMapper mapper
        )
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(int.Parse(_configuration["forex-miner-thor:Max-Retries"]));
            _connectionsSecretService = connectionsSecretService;
            _mapper = mapper;
        }

        /// <summary>
        /// Do a tick and handle trading
        /// </summary>
        /// <returns>A task</returns>
        public async Task Tick()
        {
            // Collect tradeable instrument granularities
            var tradeableGranularities = GetInstrumentGranularities().Where(granularity => granularity.IsTradeable);

            // Collect latest candles for all relevant granularities for the tick time
            var tickRelevantGranularities = tradeableGranularities.Where(granularity => IsTickRelevant(granularity.Granularity));
            var instrumentTicks = new List<InstrumentWithCandles>();
            foreach (var granularity in tickRelevantGranularities)
            {
                var lastTwoCandles = await GetLastTwoCandles(granularity.Instrument.Name, granularity.Granularity);

                // Lat candle is not complete -> market is open
                if (!lastTwoCandles.ElementAt(1).Complete)
                {
                    instrumentTicks.Add(new InstrumentWithCandles()
                    {
                        InstrumentName = granularity.Instrument.Name,
                        Granularity = granularity.Granularity,
                        Candles = new List<Candle>() { _mapper.Map<Candle>(lastTwoCandles.ElementAt(0)) }
                    });
                }
            }

            if (instrumentTicks.Count() == 0)
            {
                // Do not execute engine tick
                return;
            }

            // Tick them to the engine
            var tradeSignals = await TickToEngine(instrumentTicks);

            // Save trade signals generated
            var tradeSignalsToDb = _mapper.Map<IEnumerable<Database.Trade.TradeSignal>>(tradeSignals);
            _dbContext.AddRange(tradeSignalsToDb);
            _dbContext.SaveChanges();

            // Make a trade for each for each connection
            var connections = GetConnections();
            foreach (var conn in connections)
            {
                // Oanda
                if (conn.Broker == DbConnections.Broker.Oanda)
                {
                    var oandaServer = conn.Type == ContractConnections.ConnectionType.Demo ? OandaServer.FxPractice : OandaServer.FxTrade;
                    var oandaConnection = new ApiConnection(oandaServer, await _connectionsSecretService.GetConnectionSecret(conn.Id));

                    foreach (var ts in tradeSignals)
                    {
                        var value = 10 * (ts.Direction == TradeDirection.Long ? 1 : -1);
                        await oandaConnection.GetAccount(conn.ExternalAccountId).OpenTrade(_mapper.Map<GeriRemenyi.Oanda.V20.Client.Model.InstrumentName>(ts.Instrument), value, .01);
                    }
                }
            }
        }

        private async Task<IEnumerable<TradeSignal>> TickToEngine(IEnumerable<InstrumentWithCandles> instrumentTicks)
        {
            var client = _httpClientFactory.CreateClient(_configuration["forex-miner-thor:Name"]);
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await client.PostAsync($"tick", CreateHttpContent(instrumentTicks));

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    return await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<TradeSignal>>(responseStream);
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorResponse);
                }
            });
        }

        /// <summary>
        /// Craete HttpContent out of candles
        /// </summary>
        /// <param name="candles">The candles to convert to HttpContent</param>
        /// <returns>THe HttpContent</returns>
        private HttpContent CreateHttpContent(IEnumerable<InstrumentWithCandles> instrumentTicks)
        {
            HttpContent httpContent = null;
            if (instrumentTicks != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(instrumentTicks, ms);
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
        public void SerializeJsonIntoStream(IEnumerable<InstrumentWithCandles> instrumentTicks, Stream stream)
        {
            using var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);
            using var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None };
            var js = new Newtonsoft.Json.JsonSerializer();
            js.Serialize(jtw, instrumentTicks);
            jtw.Flush();
        }

        private async Task<IEnumerable<Oanda.Candlestick>> GetLastTwoCandles(InstrumentName instrument, Granularity granularity)
        {
            // Authenticate to OANDA with the master account
            ApiConnection oandaConnection = new ApiConnection(OandaServer.FxPractice, _configuration["Oanda-MasterToken"]);

            // Get last two candles
            var oandaInstrument = _mapper.Map<Oanda.InstrumentName>(instrument);
            var oandaGranularity = _mapper.Map<Oanda.CandlestickGranularity>(granularity);
            var candles = await oandaConnection.GetInstrument(oandaInstrument).GetLastCandles(oandaGranularity, 2);

            return candles.OrderBy(candle => candle.Time);
        }

        private IEnumerable<Database.Instrument.InstrumentGranularity> GetInstrumentGranularities()
        {
            return _dbContext.Instruments
                .Include(instrument => instrument.Granularities)
                .SelectMany(instrument => instrument.Granularities)
                .Include(granularity => granularity.Instrument);
        }

        private IEnumerable<DbConnections.Connection> GetConnections()
        {
            return _dbContext.Connections;
        }

        private bool IsTickRelevant(Granularity granularity)
        {
            var utcNow = DateTime.UtcNow;
            return granularity switch
            {
                Granularity.M1 => true,
                Granularity.M5 => utcNow.Minute % 5 == 0,
                Granularity.M10 => utcNow.Minute % 10 == 0,
                Granularity.M15 => utcNow.Minute % 15 == 0,
                Granularity.M30 => utcNow.Minute % 30 == 0,
                Granularity.H1 => utcNow.Minute == 0,
                Granularity.H2 => utcNow.Hour % 2 == 0 && utcNow.Minute == 0,
                Granularity.H3 => utcNow.Hour % 3 == 0 && utcNow.Minute == 0,
                Granularity.H4 => utcNow.Hour % 4 == 0 && utcNow.Minute == 0,
                Granularity.H6 => utcNow.Hour % 6 == 0 && utcNow.Minute == 0,
                Granularity.H8 => utcNow.Hour % 8 == 0 && utcNow.Minute == 0,
                Granularity.H12 => utcNow.Hour % 12 == 0 && utcNow.Minute == 0,
                Granularity.D => utcNow.Hour == 0 && utcNow.Minute == 0,
                _ => utcNow.Day == 1 && utcNow.Hour == 0 && utcNow.Minute == 0,
            };
        }
    }
}
