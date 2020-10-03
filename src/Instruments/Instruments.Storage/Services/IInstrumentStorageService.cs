namespace ForexMiner.Heimdallr.Instruments.Storage.Services
{
    using GeriRemenyi.Oanda.V20.Client.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Candlestick = Model.Candlestick;

    public interface IInstrumentStorageService
    {
        public Task<IEnumerable<Candlestick>> GetMonthlyData(InstrumentName instrument, CandlestickGranularity granularity, int year, int month);

        public Task AddMonthlyData(InstrumentName instrument, CandlestickGranularity granularity, int year, int month, IEnumerable<Model.Candlestick> data);
    }
}
