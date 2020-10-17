namespace ForexMiner.Heimdallr.Instruments.Api.Services
{
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using ForexMiner.Heimdallr.Common.Data.Database.Models;
    using System.Collections.Generic;

    public class InstrumentService : IInstrumentService
    {
        private ForexMinerHeimdallrDbContext _dbContext;

        public InstrumentService(ForexMinerHeimdallrDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Instrument> GetAllInstruments()
        {
            return _dbContext.Instruments;
        }
    }
}
