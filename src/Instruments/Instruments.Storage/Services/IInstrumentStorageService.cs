//----------------------------------------------------------------------------------------
// <copyright file="IInstrumentStorageService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Storage.Services
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Instrument storage service interface
    /// </summary>
    public interface IInstrumentStorageService
    {

        /// <summary>
        /// Get instrument candles with specific granularity for a timeperiod
        /// </summary>
        /// <param name="instrument">Name of the instrument</param>
        /// <param name="granularity">Granularity of the instrument</param>
        /// <param name="from">From time</param>
        /// <param name="to">To time</param>
        /// <returns>An instrument object with all candles in the requested timezone</returns>
        public Task<InstrumentWithCandles> GetInstrumentCandles(InstrumentName instrument, Granularity granularity, DateTime utcFrom, DateTime utcTo);
        
        /// <summary>
        /// Store an instrument's candles
        /// </summary>
        /// <param name="instrument">Instrument object with it's candles</param>
        public Task StoreInstrumentCandles(InstrumentWithCandles instrument);
    }
}
