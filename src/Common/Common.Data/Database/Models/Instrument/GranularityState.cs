//----------------------------------------------------------------------------------------
// <copyright file="GranularityState.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Models.Instrument
{
    /// <summary>
    /// Possible granularity states
    /// </summary>
    public enum  GranularityState
    {
        /// <summary>
        /// The granularity was just added
        /// </summary>
        New,

        /// <summary>
        /// The granularity was picked up and data loading is in progress
        /// </summary>
        InDataLoading,

        /// <summary>
        /// The granularity data is present and tradeable
        /// </summary>
        Tradeable,

        /// <summary>
        /// The granularity is removed data is beeing deleted
        /// </summary>
        InDelete
    }
}
