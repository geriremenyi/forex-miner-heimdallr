namespace ForexMiner.Heimdallr.Instruments.Worker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ForexMiner.Heimdallr.Common.Data.Instrument;
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using GeriRemenyi.Oanda.V20.Sdk;
    using GeriRemenyi.Oanda.V20.Sdk.Utilities;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Candlestick = ForexMiner.Heimdallr.Instruments.Storage.Model.Candlestick;
    using CandlestickData = ForexMiner.Heimdallr.Instruments.Storage.Model.CandlestickData;

    public class Worker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IInstrumentStorageService _storageService;

        public Worker(IConfiguration configuration, IInstrumentStorageService storageService)
        {
            _configuration = configuration;
            _storageService = storageService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Authenticate to OANDA with the master account
                ApiConnection oandaConnection = new ApiConnection(OandaServer.FxPractice, _configuration["Oanda:MasterToken"]);

                // Collect historical data for instruments supported
                IEnumerable<InstrumentDTO> supportedInstruments = new List<InstrumentDTO>(){
                    new InstrumentDTO() { InstrumentName = InstrumentName.EUR_USD, IsTradeable = true }
                }; // TODO: get this from the instruments API

                foreach (InstrumentDTO instrument in supportedInstruments)
                {
                    var inst = oandaConnection.GetInstrument(instrument.InstrumentName);

                    // TODO: get this from config
                    DateTime startDate = new DateTime(2010, 01, 01);
                    DateTime endDate = new DateTime(2020, 09, 30);

                    // Store hourly candles day by day
                    DateTime currentMonth = startDate;
                    while (currentMonth <= endDate) {
                        var candlesResponse = await inst.GetCandles(CandlestickGranularity.H1, currentMonth, new DateTime(currentMonth.Year, currentMonth.Month, DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month)));
                        var candles = candlesResponse.Candles;
                        await _storageService.AddMonthlyData(InstrumentName.EUR_USD, CandlestickGranularity.H1, currentMonth.Year, currentMonth.Month, candles.Select(
                            c => new Candlestick() {
                                Time = Convert.ToDateTime(c.Time),
                                Volume = c.Volume,
                                // TODO use Ask and Bid instead
                                Ask = new CandlestickData() {
                                    Open = c.Mid.O,
                                    Close = c.Mid.C,
                                    Low = c.Mid.L,
                                    High = c.Mid.H
                                },
                                Bid = new CandlestickData()
                                {
                                    Open = c.Mid.O,
                                    Close = c.Mid.C,
                                    Low = c.Mid.L,
                                    High = c.Mid.H
                                },
                            })
                        );
                        currentMonth = currentMonth.AddMonths(1);
                    }
                }


                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
