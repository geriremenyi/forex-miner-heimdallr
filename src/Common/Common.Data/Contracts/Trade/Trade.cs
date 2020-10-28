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

    /// <summary>
    /// Trade data transfer object
    /// </summary>
    public class Trade
    {
        /// <summary>
        /// Unique identifier of the trade
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Instrument the trade was executed on
        /// </summary>
        public InstrumentName Instrument { get; set; }

        /// <summary>
        /// Price the trade was opened at
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Time of the trade opened
        /// </summary>
        public DateTime OpenTime { get; set;}

        /// <summary>
        /// Numnber of units open
        /// </summary>
        public double CurrentUnits { get; set; }
    }
}
