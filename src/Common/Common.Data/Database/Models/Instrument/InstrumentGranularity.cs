//----------------------------------------------------------------------------------------
// <copyright file="InstrumentGranularity.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Instrument's granularity and it's state
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InstrumentGranularity
    {

        /// <summary>
        /// Key of the instrument granularity
        /// Required because only with fluent API you can create composite primary keys
        /// which I quite don't like so introducing globally unique identifier
        /// https://github.com/dotnet/efcore/issues/11003
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Instrument
        /// </summary>
        public Instrument Instrument { get; set; }

        /// <summary>
        /// Granularity name
        /// </summary>
        public Granularity Granularity { get; set; }

        /// <summary>
        /// Granularity state
        /// </summary>
        public GranularityState State { get; set; }

        /// <summary>
        /// Is the instrument tradeable
        /// </summary>
        public bool IsTradeable { get { return State == GranularityState.Tradeable; } }
    }
}
