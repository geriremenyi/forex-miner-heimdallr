namespace ForexMiner.Heimdallr.Instruments.Storage.Model
{
    using GeriRemenyi.Oanda.V20.Client.Model;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    public class Instrument
    {
        public InstrumentName Name { get; set; }

        public Granularity Granularity { get; set; }

        public DateTime From { get; set; }
        
        public DateTime To { get; set; }

        public List<Candle> Candles { get; set; }

        public Instrument(InstrumentName name, Granularity granularity, DateTime from, DateTime to)
        {
            Name = name;
            Granularity = granularity;
            From = from;
            To = to;
            Candles = new List<Candle>();
        }
    }
}
