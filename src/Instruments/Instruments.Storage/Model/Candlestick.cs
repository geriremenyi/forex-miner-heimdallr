namespace ForexMiner.Heimdallr.Instruments.Storage.Model
{
    using System;
    public class Candlestick
    {
        public DateTime Time { get; set; }

        public long Volume { get; set; }

        public CandlestickData Ask { get; set; }

        public CandlestickData Bid { get; set; }
    }
}
