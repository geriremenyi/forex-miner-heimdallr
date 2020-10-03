namespace ForexMiner.Heimdallr.Instruments.Api.Controllers.V1
{
    using ForexMiner.Heimdallr.Instruments.Api.Database;
    using ForexMiner.Heimdallr.Instruments.Api.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/instruments")]
    public class InstrumentController : ControllerBase
    {
        private IInstrumentService _instrumentService;

        public InstrumentController(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }

        public IEnumerable<Instrument> GetAllInstruments()
        {
            return _instrumentService.GetAllInstruments();
        }

    }
}
