//----------------------------------------------------------------------------------------
// <copyright file="InstrumentGranularityCreation.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Instrument granularity contract
    /// </summary>
    [DataContract]
    public class InstrumentGranularityCreation
    {
        /// <summary>
        /// Granularity name
        /// </summary>
        [DataMember(Name = "granularity")]
        public Granularity Granularity { get; set; }
    }
}
