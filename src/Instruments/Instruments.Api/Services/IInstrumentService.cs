namespace ForexMiner.Heimdallr.Instruments.Api.Services
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models;
    using System.Collections.Generic;
    using System.Linq;

    public interface IInstrumentService
    {
        public IEnumerable<Instrument> GetAllInstruments();
    }
}
