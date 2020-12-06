//----------------------------------------------------------------------------------------
// <copyright file="TradeSignal.cs" company="geriremenyi.com">
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
    /// Representation of a trade signal
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class TradeSignal
    {
        /// <summary>
        /// Time the trade signal was created
        /// </summary>
        [DataMember(Name = "time")]
        [Required]
        public DateTime Time { get; set; }

        /// <summary>
        /// Instrument of the trade signal
        /// </summary>
        [DataMember(Name = "instrument")]
        [Required]
        public InstrumentName Instrument { get; set; }

        /// <summary>
        /// Direction of the trade signal
        /// Either LONG or SHORT
        /// </summary>
        [DataMember(Name = "direction")]
        [Required]
        public TradeDirection Direction { get; set; }

        /// <summary>
        /// Confidence of the trade signal
        /// </summary>
        [DataMember(Name = "confidence")]
        [Required]
        public double Confidence { get; set; }

        /// <summary>
        /// Constructor for the trade signal
        /// Sets the signal date to current time
        /// </summary>
        public TradeSignal()
        {
            Time = DateTime.UtcNow;
        }
    }
}
