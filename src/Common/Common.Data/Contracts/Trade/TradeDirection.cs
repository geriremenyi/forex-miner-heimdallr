//----------------------------------------------------------------------------------------
// <copyright file="TradeDirection.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Trade
{
    /// <summary>
    /// Direction of a trade
    /// 
    /// Either long (a.k.a.: buy) or short (a.k.a.: sell)
    /// </summary>
    public enum TradeDirection
    {
        /// <summary>
        /// Buy direction
        /// </summary>
        Long,

        /// <summary>
        /// Sell direction
        /// </summary>
        Short
    }
}
