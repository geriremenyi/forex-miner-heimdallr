namespace ForexMiner.Heimdallr.Instruments.Worker.Services.History
{
    using ForexMiner.Heimdallr.Instruments.Storage.Model;
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using GeriRemenyi.Oanda.V20.Sdk;
    using GeriRemenyi.Oanda.V20.Sdk.Utilities;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Polly;
    using Polly.Retry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Candlestick = Storage.Model.Candlestick;
    using Instrument = Storage.Model.Instrument;

    class HistoryService : IHistoryService
    {
        private static readonly DateTime START_TIME = new DateTime(2005, 1, 1);

        private static readonly int MAX_RETRIES = 5;

        private readonly IConfiguration _configuration;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly AsyncRetryPolicy<IEnumerable<Common.Data.Contracts.Instrument.Instrument>> _retryPolicy;

        private readonly IInstrumentStorageService _instrumentStorageService;

        public HistoryService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IInstrumentStorageService instrumentStorageService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _retryPolicy = Policy<IEnumerable<Common.Data.Contracts.Instrument.Instrument>>.Handle<HttpRequestException>().RetryAsync(MAX_RETRIES);
            _instrumentStorageService = instrumentStorageService;
        }

        public async Task CheckAndFillToday() 
        {
            var now = DateTime.Now;
            var todayMorning = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            await CheckAndFill(todayMorning, now);
        }

        public async Task CheckAndFillUntilYesterday() 
        {
            var now = DateTime.Now;
            var yesterdayEvening = new DateTime(now.Year, now.Month, now.AddDays(-1).Day, 23, 59, 59);
            var beginningOfTime = START_TIME;
            await CheckAndFill(beginningOfTime, yesterdayEvening);
        }

        private async Task CheckAndFill(DateTime startTime, DateTime endTime)
        {
            // Collect historical data for instruments supported
            var supportedInstruments = await GetSupportedInstruments();

            // Store candles in monthly buckets for all granularities
            // for the supported instruments
            foreach (var instrument in supportedInstruments)
            {
                foreach (Granularity granularity in Enum.GetValues(typeof(Granularity)))
                {
                    var beginningOfTheMonth = new DateTime(startTime.Year, startTime.Month, 1);
                    while (beginningOfTheMonth <= endTime)
                    {
                        var endOfTheMonth = new DateTime(beginningOfTheMonth.Year, beginningOfTheMonth.Month, beginningOfTheMonth.AddMonths(1).AddDays(-1).Day);
                        if (endOfTheMonth > endTime)
                        {
                            endOfTheMonth = endTime;
                        }

                        try
                        {
                            var instrumentCandles = await _instrumentStorageService.GetInstrumentCandles(
                                instrument.Name, 
                                granularity,
                                beginningOfTheMonth,
                                endOfTheMonth
                            );
                            var instrumentCandlesOrdered = instrumentCandles.Candles.OrderBy(c => c.Time);

                            if (instrumentCandlesOrdered.First().Time.GetInclusiveCandleTime(granularity) != startTime 
                                || instrumentCandlesOrdered.Last().Time.GetInclusiveCandleTime(granularity) != endTime)
                            {
                                // TODO: more meaningful exception
                                //throw new Exception();
                            }
                        }
                        catch
                        {
                            // Authenticate to OANDA with the master account
                            ApiConnection oandaConnection = new ApiConnection(OandaServer.FxPractice, _configuration["Oanda:MasterToken"]);

                            var oandaCandles = (await oandaConnection.GetInstrument(instrument.Name).GetCandles(
                                CandlestickGranularity.H1,
                                beginningOfTheMonth,
                                endOfTheMonth // TODO: at minutes it will only load 5000, handle it gracefully in the SDK
                            )).Candles;

                            var instrumentInPlay = new Instrument(
                                instrument.Name,
                                granularity,
                                beginningOfTheMonth,
                                endOfTheMonth
                            );
                            // TODO: automapper
                            instrumentInPlay.Candles.AddRange(oandaCandles.Select(c => new Candle()
                            {
                                Time = Convert.ToDateTime(c.Time),
                                Volume = c.Volume,   
                                Bid = new Candlestick()
                                {
                                    Open = c.Bid.O,
                                    Close = c.Bid.C,
                                    Low = c.Bid.L,
                                    High = c.Bid.H
                                },
                                Mid = new Candlestick()
                                {
                                    Open = c.Mid.O,
                                    Close = c.Mid.C,
                                    Low = c.Mid.L,
                                    High = c.Mid.H
                                },
                                Ask = new Candlestick()
                                {
                                    Open = c.Ask.O,
                                    Close = c.Ask.C,
                                    Low = c.Ask.L,
                                    High = c.Ask.H
                                }
                            }));
                            await _instrumentStorageService.StoreInstrumentCandles(instrumentInPlay);
                        }

                        beginningOfTheMonth = beginningOfTheMonth.AddMonths(1);
                    }
                }
            }
        }

        private async Task<IEnumerable<Common.Data.Contracts.Instrument.Instrument>> GetSupportedInstruments()
        {
            var client = _httpClientFactory.CreateClient(_configuration["ForexMiner.Heimdallr.Instruments.Api:Name"]);
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                
                var response = await client.GetAsync("");
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Common.Data.Contracts.Instrument.Instrument>>(responseBody);
            });
        }
    }
}
