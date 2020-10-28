//----------------------------------------------------------------------------------------
// <copyright file="Instrument.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Representation of an instrument and it's handled granularities
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Instrument
    {
        /// <summary>
        /// Name of the instrument
        /// </summary>
        [Key]
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
