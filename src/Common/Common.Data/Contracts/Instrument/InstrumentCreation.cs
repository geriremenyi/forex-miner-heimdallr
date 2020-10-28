//----------------------------------------------------------------------------------------
// <copyright file="InstrumentCreation.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Instrument data transfer object
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class InstrumentCreation
    {
        /// <summary>
        /// Name of the instrument
        /// </summary>
        [DataMember(Name = "name")]
        public InstrumentName Name { get; set; }
    }
}
