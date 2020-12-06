//----------------------------------------------------------------------------------------
// <copyright file="Trade.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Trade
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Trade data transfer object
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class Trade
    {
        /// <summary>
        /// Unique identifier of the trade
        /// </summary>
        [DataMember(Name = "id")]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Instrument the trade was executed on
        /// </summary>
        [DataMember(Name = "instrument")]
        [Required]
        public InstrumentName Instrument { get; set; }

        /// <summary>
        /// Price the trade was opened at
        /// </summary>
        [DataMember(Name = "price")]
        [Required]
        public double Price { get; set; }

        /// <summary>
        /// Time of the trade opened
        /// </summary>
        [DataMember(Name = "openTime")]
        [Required]
        public DateTime OpenTime { get; set;}

        /// <summary>
        /// Numnber of units open
        /// </summary>
        [DataMember(Name = "currentUnits")]
        [Required]
        public double CurrentUnits { get; set; }
    }
}
