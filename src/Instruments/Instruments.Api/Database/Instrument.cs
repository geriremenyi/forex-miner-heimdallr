namespace ForexMiner.Heimdallr.Instruments.Api.Database
{
    using GeriRemenyi.Oanda.V20.Client.Model;
    using System.ComponentModel.DataAnnotations;

    public class Instrument
    {
        [Key]
        public InstrumentName InstrumentName { get; set; }

        [Required]
        public bool IsTradeable { get; set; }
    }
}
