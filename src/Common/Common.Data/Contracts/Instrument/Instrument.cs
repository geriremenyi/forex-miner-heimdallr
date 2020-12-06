//----------------------------------------------------------------------------------------
// <copyright file="InstrumentCreation.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Representation of an instrument
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class Instrument
    {
        /// <summary>
        /// Name of the instrument
        /// </summary>
        [DataMember(Name = "name")]
        public InstrumentName Name { get; set; }

        /// <summary>
        /// Instrument's granularities
        /// </summary>
        [DataMember(Name = "granularities")]
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
