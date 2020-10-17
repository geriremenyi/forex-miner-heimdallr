//----------------------------------------------------------------------------------------
// <copyright file="Instrument.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------
namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    using GeriRemenyi.Oanda.V20.Client.Model;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Instrument data transfer object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Instrument
    {
        /// <summary>
        /// Name of the instrument
        /// </summary>
        public InstrumentName Name { get; set; }
        
        /// <summary>
        /// Is the instrument tradeable through the applicatiomn
        /// </summary>
        public bool IsTradeable { get; set; }

        /// <summary>
        /// How many times were the instrument traded through the application
        /// </summary>
        public int TradedCounter { get; set; }
    }
}
