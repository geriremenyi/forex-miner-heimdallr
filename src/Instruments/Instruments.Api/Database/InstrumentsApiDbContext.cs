namespace ForexMiner.Heimdallr.Instruments.Api.Database
{
    using Microsoft.EntityFrameworkCore;

    public class InstrumentsApiDbContext : DbContext
    {
        public DbSet<Instrument> Instruments { get; set; }

        public InstrumentsApiDbContext(DbContextOptions<InstrumentsApiDbContext> options) : base(options) { }
    }

}
