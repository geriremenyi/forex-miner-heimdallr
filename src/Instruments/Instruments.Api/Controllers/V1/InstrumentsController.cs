//----------------------------------------------------------------------------------------
// <copyright file="InstrumentsController.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Api.Controllers.V1
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Instruments.Api.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    /// <summary>
    /// Instrument controller
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/instruments")]
    public class InstrumentsController : ControllerBase
    {
        /// <summary>
        /// Instrument service
        /// </summary>
        private IInstrumentService _instrumentService;

        /// <summary>
        /// Instriment controller constructor.
        /// Sets up the instrument service
        /// </summary>
        /// <param name="instrumentService"></param>
        public InstrumentsController(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public Instrument AddInstrument([FromBody] InstrumentCreation instrument)
        {
            return _instrumentService.AddInstrument(instrument);
        }

        
        /// <summary>
        /// Get instrument endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<Instrument> GetAllInstrument()
        {
            return _instrumentService.GetAllInstruments();
        }

        /// <summary>
        /// Add granularity endpoint
        /// </summary>
        /// <param name="instrument">Instrument to add the granularity to</param>
        /// <param name="granularity">Granularity to add</param>
        /// <returns>The instrument which the granularity was added to</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("{instrument}/granularities")]
        public Instrument AddGranularity([FromRoute] InstrumentName instrument, [FromBody] InstrumentGranularityCreation granularity)
        {
            return _instrumentService.AddGranularity(instrument, granularity);
        }
    }
}
