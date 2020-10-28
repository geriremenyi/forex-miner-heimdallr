//----------------------------------------------------------------------------------------
// <copyright file="InstrumentTick.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Representation of an instrument tick
    /// </summary>
    [DataContract]
    public class InstrumentWithCandles
    {
        /// <summary>
        /// Name of the instrument
        /// </summary>
        [DataMember(Name = "instrument")]
        [JsonConverter(typeof(StringEnumConverter))] 
        public InstrumentName InstrumentName { get; set; }

        /// <summary>
        /// Granularity
        /// </summary>
        [DataMember(Name = "granularity")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Granularity Granularity { get; set; }

        /// <summary>
        /// Last candles
        /// </summary>
        [DataMember(Name = "candles")]
        public ICollection<Candle> Candles { get; set; }
    }
}
