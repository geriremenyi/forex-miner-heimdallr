//----------------------------------------------------------------------------------------
// <copyright file="ConnectionCreation.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Connection
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Representation of a connection to create
    /// </summary>
    [DataContract]
    public enum ConnectionType
    {
        /// <summary>
        /// The connection is connecting to a demo/practice environment
        /// </summary>
        [EnumMember(Value = "demo")]
        Demo,

        /// <summary>
        /// The connection is connecting to a live trading environment
        /// </summary>
        [EnumMember(Value = "live")]
        Live
    }
}
