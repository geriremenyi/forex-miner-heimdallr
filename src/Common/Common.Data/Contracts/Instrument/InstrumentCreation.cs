//----------------------------------------------------------------------------------------
// <copyright file="InstrumentCreation.cs" company="geriremenyi.com">
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
    public class InstrumentCreation
    {
        /// <summary>
        /// Name of the instrument
        /// </summary>
        public InstrumentName Name { get; set; }
    }
}
