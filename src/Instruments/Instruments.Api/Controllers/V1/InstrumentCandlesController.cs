namespace ForexMiner.Heimdallr.Instruments.Api.Controllers.V1
{
    using ForexMiner.Heimdallr.Instruments.Storage.Model;
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Instrument = Storage.Model.Instrument;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/instruments/{instrument}/candles/{granularity}")]
    public class InstrumentCandlesController : ControllerBase
    {
        IInstrumentStorageService _instrumentStorageService;

        public InstrumentCandlesController(IInstrumentStorageService instrumentStorageService)
        {
            _instrumentStorageService = instrumentStorageService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<Instrument> GetInstrumentCandles(
            [FromRoute] InstrumentName instrument,
            [FromRoute] Granularity granularity, 
            [FromQuery] DateTime from, 
            [FromQuery] DateTime to
        )
        {
            return await _instrumentStorageService.GetInstrumentCandles(instrument, granularity, from, to);
        }

    }
}
