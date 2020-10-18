//----------------------------------------------------------------------------------------
// <copyright file="Instrument.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Models
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Connection;
    using GeriRemenyi.Oanda.V20.Client.Model;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Representation of a trade in the database
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Trade
    {
        /// <summary>
        /// Unique identifier of the trade
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// External broker connection where the trade was executed
        /// </summary>
        [Required]
        public BrokerConnection TradeExternalSource { get; set; }

        /// <summary>
        /// External identifier of the trade in the external broker's system
        /// </summary>
        [Required]
        public Guid TradeExternalId { get; set; }

        /// <summary>
        /// Instrument the trade was executed on
        /// </summary>
        [Required]
        public InstrumentName Instrument { get; set; }
    }
}
