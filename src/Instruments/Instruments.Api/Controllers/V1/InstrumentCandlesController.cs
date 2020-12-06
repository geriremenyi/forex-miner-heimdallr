//----------------------------------------------------------------------------------------
// <copyright file="InstrumentCandlesController.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Api.Controllers.V1
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Instrument candles controller
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/instruments/{instrument}/granularities/{granularity}/candles")]
    public class InstrumentCandlesController : ControllerBase
    {
        /// <summary>
        /// The instrument storage service
        /// </summary>
        private readonly IInstrumentStorageService _instrumentStorageService;

        /// <summary>
        /// Instrument candles controller constructor
        /// Sets up the required service
        /// </summary>
        /// <param name="instrumentStorageService">The storage service where the instruments are stored</param>
        public InstrumentCandlesController(IInstrumentStorageService instrumentStorageService)
        {
            _instrumentStorageService = instrumentStorageService;
        }

        /// <summary>
        /// Get instrument candles for specific time-range
        /// Not bound to an user but only allow users to query this endpoint
        /// </summary>
        /// <param name="instrument">Instrument name</param>
        /// <param name="granularity">Granularity</param>
        /// <param name="from">Time from</param>
        /// <param name="to">Time to</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<InstrumentWithCandles> GetInstrumentCandles(
            [FromRoute] InstrumentName instrument,
            [FromRoute] Granularity granularity, 
            [FromQuery] DateTime from, 
            [FromQuery] DateTime to
        )
        {
            return await _instrumentStorageService.GetInstrumentCandles(instrument, granularity, from.ToUniversalTime(), to.ToUniversalTime());
        }

    }
}
