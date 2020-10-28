//----------------------------------------------------------------------------------------
// <copyright file="IInstrumentService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Api.Services
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Instrument service interface
    /// </summary>
    public interface IInstrumentService
    {
        /// <summary>
        /// Get all instruments
        /// </summary>
        /// <returns>The list of all instruments</returns>
        public IEnumerable<Instrument> GetAllInstruments();

        /// <summary>
        /// Add a new instrument to the tradeable instruments
        /// </summary>
        /// <param name="instrument">The instrument to add to tradeable</param>
        /// <returns>The instrument added</returns>
        public Instrument AddInstrument(InstrumentCreation instrument);


        /// <summary>
        /// Add a new supported granularity to an existing instrument
        /// </summary>
        /// <param name="instrumentName">Name of the instrument to add the granularity to</param>
        /// <param name="granularity">Granularity to add</param>
        /// <returns>The whole instrument with the new granularity added</returns>
        public Instrument AddGranularity(InstrumentName instrumentName, InstrumentGranularityCreation granularity);
    }
}
