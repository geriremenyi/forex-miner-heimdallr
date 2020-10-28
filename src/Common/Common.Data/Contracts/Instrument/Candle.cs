//----------------------------------------------------------------------------------------
// <copyright file="Candle.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Representation of a japanese candle with it's time and volume
    /// </summary>
    [DataContract]
    public class Candle
    {

        /// <summary>
        /// UTC timestamp of the candle start time
        /// </summary>
        [DataMember(Name = "time")]
        public DateTime Time { get; set; }

        /// <summary>
        /// Trading volume
        /// </summary>
        [DataMember(Name = "volume")]
        public long Volume { get; set; }

        /// <summary>
        /// Mid candlestick
        /// </summary>
        [DataMember(Name = "bid")]
        public Candlestick Bid { get; set; }

        /// <summary>
        /// Mid candlestick
        /// </summary>
        [DataMember(Name = "mid")]
        public Candlestick Mid { get; set; }

        /// <summary>
        /// Ask candlestick
        /// </summary>
        [DataMember(Name = "ask")]
        public Candlestick Ask { get; set; }
    }
}
