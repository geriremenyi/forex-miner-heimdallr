namespace ForexMiner.Heimdallr.Instruments.Api.Services
{
    using ForexMiner.Heimdallr.Instruments.Api.Database;
    using System.Collections.Generic;
    using System.Linq;

    public interface IInstrumentService
    {
        public IEnumerable<Instrument> GetAllInstruments();
    }
}
