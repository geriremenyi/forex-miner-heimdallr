//----------------------------------------------------------------------------------------
// <copyright file="TradeDirection.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Trade
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Direction of a trade
    /// 
    /// Either long (a.k.a.: buy) or short (a.k.a.: sell)
    /// </summary>
    [DataContract]
    public enum TradeDirection
    {
        /// <summary>
        /// Buy direction
        /// </summary>
        [EnumMember(Value = "long")]
        Long,

        /// <summary>
        /// Sell direction
        /// </summary>
        [EnumMember(Value = "short")]
        Short
    }
}
