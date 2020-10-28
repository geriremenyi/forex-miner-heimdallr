//----------------------------------------------------------------------------------------
// <copyright file="Connection.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Models.Connection
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Connection;
    using ForexMiner.Heimdallr.Common.Data.Database.Models.User;
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Representation of a connections
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Connection
    {
        /// <summary>
        /// Id of the connection
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User defined name of the connection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the connection
        /// </summary>
        public Broker Broker { get; set; }

        /// <summary>
        /// Type of the connection
        /// Either demo or live
        /// </summary>
        public ConnectionType Type { get; set; }

        /// <summary>
        /// Account ID to use for the external connection
        /// </summary>
        public string ExternalAccountId { get; set; }

        /// <summary>
        /// Owner user of the connection
        /// </summary>
        public User Owner { get; set; }
    }
}
