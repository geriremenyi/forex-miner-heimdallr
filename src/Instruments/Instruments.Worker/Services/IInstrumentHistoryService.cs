//----------------------------------------------------------------------------------------
// <copyright file="IHistoryService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Worker.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// History service interface
    /// </summary>
    public interface IInstrumentHistoryService
    {
        /// <summary>
        /// Check if there are any new instruments granularities present in the DB
        /// and if yes then load it's historical data
        /// </summary>
        /// <returns>A async task</returns>
        public Task CheckInstrumentGranularitiesAndLoadData();
    }
}
