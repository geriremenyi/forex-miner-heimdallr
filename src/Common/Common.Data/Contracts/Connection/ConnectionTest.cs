//----------------------------------------------------------------------------------------
// <copyright file="ConnectionCreation.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Connection
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models.Connection;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Representation of a connection to create
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class ConnectionTest
    {
        /// <summary>
        /// Type of the connection
        /// </summary>
        [DataMember(Name = "broker")]
        public Broker Broker { get; set; }

        /// <summary>
        /// Secret for the connection
        /// </summary>
        [DataMember(Name = "secret")]
        public string Secret { get; set; }
    }
}
