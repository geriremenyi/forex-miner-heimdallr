//----------------------------------------------------------------------------------------
// <copyright file="InstrumentCreation.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    using System.Collections.Generic;

    /// <summary>
    /// Representation of an instrument
    /// </summary>
    public class Instrument
    {
        /// <summary>
        /// Name of the instrument
        /// </summary>
        public InstrumentName Name { get; set; }

        /// <summary>
        /// Instrument's granularities
        /// </summary>
        public ICollection<InstrumentGranularity> Granularities { get; set; }

        /// <summary>
        /// Instrument constructor
        /// Initializes and empty granularities list
        /// </summary>
        public Instrument()
        {
            Granularities = new List<InstrumentGranularity>();
        }
    }
}
