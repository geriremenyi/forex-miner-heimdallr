//----------------------------------------------------------------------------------------
// <copyright file="Granularity.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Supported granularities within the application
    /// </summary>
    [DataContract]
    public enum Granularity
    {
        /// <summary>
        /// 1 minute
        /// </summary>
        [EnumMember(Value = "M1")]
        M1,

        /// <summary>
        /// 5 minutes
        /// </summary>
        [EnumMember(Value = "M5")]
        M5,

        /// <summary>
        /// 10 minutes
        /// </summary>
        [EnumMember(Value = "M10")]
        M10,

        /// <summary>
        /// 15 minutes
        /// </summary>
        [EnumMember(Value = "M15")]
        M15,

        /// <summary>
        /// 30 minutes
        /// </summary>
        [EnumMember(Value = "M30")]
        M30,

        /// <summary>
        /// 1 hour
        /// </summary>
        [EnumMember(Value = "H1")]
        H1,

        /// <summary>
        /// 2 hours
        /// </summary>
        [EnumMember(Value = "H2")]
        H2,

        /// <summary>
        /// 3 hours
        /// </summary>
        [EnumMember(Value = "H3")]
        H3,

        /// <summary>
        /// 4 hours
        /// </summary>
        [EnumMember(Value = "H4")]
        H4,

        /// <summary>
        /// 6 hours
        /// </summary>
        [EnumMember(Value = "H6")]
        H6,

        /// <summary>
        /// 8 hours
        /// </summary>
        [EnumMember(Value = "H8")]
        H8,

        /// <summary>
        /// 12 hours
        /// </summary>
        [EnumMember(Value = "H12")]
        H12,

        /// <summary>
        /// 1 day
        /// </summary>
        [EnumMember(Value = "D")]
        D,

        /// <summary>
        /// 1 month
        /// </summary>
        [EnumMember(Value = "M")]
        M
    }
}
