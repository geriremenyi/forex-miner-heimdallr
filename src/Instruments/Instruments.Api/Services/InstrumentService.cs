namespace ForexMiner.Heimdallr.Instruments.Api.Services
{
    using ForexMiner.Heimdallr.Instruments.Api.Database;
    using System.Collections.Generic;

    public class InstrumentService : IInstrumentService
    {
        private InstrumentsApiDbContext _context;

        public InstrumentService(InstrumentsApiDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Instrument> GetAllInstruments()
        {
            return _context.Instruments;
        }
    }
}
