//----------------------------------------------------------------------------------------
// <copyright file="InstrumentGranularity.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    /// <summary>
    /// Instrument granularity contract
    /// </summary>
    public class InstrumentGranularity
    {
        /// <summary>
        /// Granularity name
        /// </summary>
        public Granularity Granularity { get; set; }

        /// <summary>
        /// Is the instrument tradeable
        /// </summary>
        public bool IsTradeable { get; set; }
    }
}
