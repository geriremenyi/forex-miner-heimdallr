namespace ForexMiner.Heimdallr.Instruments.Storage.Services
{
    using ForexMiner.Heimdallr.Instruments.Storage.Model;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Instrument = Model.Instrument;

    public interface IInstrumentStorageService
    {
        public Task<Instrument> GetInstrumentCandles(InstrumentName instrument, Granularity granularity, DateTime from, DateTime to);
        public Task StoreInstrumentCandles(Instrument instrument);
    }
}
