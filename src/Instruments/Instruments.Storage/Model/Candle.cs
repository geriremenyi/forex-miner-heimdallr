namespace ForexMiner.Heimdallr.Instruments.Storage.Model
{
    using System;

    public class Candle
    {
        public DateTime Time { get; set; }

        public long Volume { get; set; }

        public Candlestick Ask { get; set; }

        public Candlestick Mid { get; set; }

        public Candlestick Bid { get; set; }
    }
}
