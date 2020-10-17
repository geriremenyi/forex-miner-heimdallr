namespace ForexMiner.Heimdallr.Instruments.Api.Controllers.V1
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models;
    using ForexMiner.Heimdallr.Instruments.Api.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/instruments")]
    public class InstrumentsController : ControllerBase
    {
        private IInstrumentService _instrumentService;

        public InstrumentsController(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<Instrument> GetAllInstruments()
        {
            var instruments = _instrumentService.GetAllInstruments();
            return instruments;
        }

    }
}
