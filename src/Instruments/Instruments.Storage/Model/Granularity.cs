namespace ForexMiner.Heimdallr.Instruments.Storage.Model
{
    /// <summary>
    /// Subset of the CandlestickGranularity from the GeriRemenyi.Oanda.V20.Client library.
    /// These granularities are supported within the forex-miner automated trading platform.
    /// </summary>
    public enum Granularity
    {
        M1,
        M5,
        M10,
        M15,
        M30,
        H1,
        H2,
        H3,
        H4,
        H6,
        H8,
        H12,
        D,
        M
    }
}
