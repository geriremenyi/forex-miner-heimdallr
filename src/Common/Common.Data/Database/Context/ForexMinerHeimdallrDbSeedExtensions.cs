namespace ForexMiner.Heimdallr.Common.Data.Database.Context
{
    using Instrument = Models.Instrument;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using Microsoft.EntityFrameworkCore;

    public static class ForexMinerHeimdallrDbSeedExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Instrument>().HasData(
                new Instrument { Name = InstrumentName.EUR_USD, IsTradeable = true  },
                new Instrument { Name = InstrumentName.USD_JPY, IsTradeable = true },
                new Instrument { Name = InstrumentName.GBP_USD, IsTradeable = true },
                new Instrument { Name = InstrumentName.USD_CHF, IsTradeable = true },
                new Instrument { Name = InstrumentName.AUD_USD, IsTradeable = true },
                new Instrument { Name = InstrumentName.USD_CAD, IsTradeable = true },
                new Instrument { Name = InstrumentName.NZD_USD, IsTradeable = true }
            );
        }
    }
}
