//----------------------------------------------------------------------------------------
// <copyright file="Candlestick.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// The usual OHLC candlestick representation
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class Candlestick
    {
        /// <summary>
        /// Open
        /// </summary>
        [DataMember(Name = "open")]
        public double Open { get; set; }

        /// <summary>
        /// High
        /// </summary>
        [DataMember(Name = "high")]
        public double High { get; set; }

        /// <summary>
        /// Low
        /// </summary>
        [DataMember(Name = "low")]
        public double Low { get; set; }

        /// <summary>
        /// Close
        /// </summary>
        [DataMember(Name = "close")]
        public double Close { get; set; }
    }
}
