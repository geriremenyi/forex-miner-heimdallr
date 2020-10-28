//----------------------------------------------------------------------------------------
// <copyright file="TradeSignal.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Models.Trade
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Instrument;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Trade;
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Representation of a trade signal in the DB
    /// </summary>
    public class TradeSignal
    {
        /// <summary>
        /// Id of the trade signal
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Time the trade signal was created
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The instrument name the trade signal was generated for
        /// </summary>
        public InstrumentName Instrument { get; set; }

        /// <summary>
        /// Signaled diretion
        /// </summary>
        public TradeDirection Direction { get; set; }

        /// <summary>
        /// Confidence of the trade signal
        /// </summary>
        public double Confidence { get; set; }
    }
}
