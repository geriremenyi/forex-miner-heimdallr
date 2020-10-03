namespace ForexMiner.Heimdallr.Common.Data.Instrument
{
    using GeriRemenyi.Oanda.V20.Client.Model;

    public class InstrumentDTO
    {
        public InstrumentName InstrumentName { get; set; }
        public bool IsTradeable { get; set; }
    }
}
