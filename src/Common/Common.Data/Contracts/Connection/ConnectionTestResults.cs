//----------------------------------------------------------------------------------------
// <copyright file="ConnectionCreation.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Connection
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models.Connection;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Representation of a connection to create
    /// </summary>
    [DataContract]
    public class ConnectionTestResults
    {
        /// <summary>
        /// Type of the connection
        /// </summary>
        [DataMember(Name = "type")]
        public ConnectionType Type { get; set; }

        /// <summary>
        /// Available account IDs under the connection
        /// </summary>
        public IEnumerable<string> AccountIds { get; set; }
    }
}
