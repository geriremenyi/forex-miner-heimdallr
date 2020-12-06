//----------------------------------------------------------------------------------------
// <copyright file="Connection.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Connection
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models.Connection;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Trade;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Representation of a connections
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class Connection
    {
        /// <summary>
        /// Id of the connection
        /// </summary>
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// User defined name of the connection
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Type of the connection
        /// </summary>
        [DataMember(Name = "broker")]
        public Broker Broker { get; set; }

        /// <summary>
        /// Type of the connection
        /// Either demo or live
        /// </summary>
        [DataMember(Name = "type")]
        public ConnectionType Type { get; set; }

        /// <summary>
        /// Account ID to use for the external connection
        /// </summary>
        [DataMember(Name = "externalAccountId")]
        public string ExternalAccountId { get; set; }

        /// <summary>
        /// Balance of the connection
        /// </summary>
        [DataMember(Name = "balance")]
        public double Balance { get; set; }

        /// <summary>
        /// Profit or loss of the connection
        /// Negative means loss, posistive means profit
        /// </summary>
        [DataMember(Name = "profitLoss")]
        public double ProfitLoss { get; set; }

        /// <summary>
        /// Trades made through the connection
        /// </summary>
        [DataMember(Name = "openTrades")]
        public IEnumerable<Trade> OpenTrades { get; set; }

        /// <summary>
        /// Connection constructor
        /// Initializes and empty list for the trades
        /// </summary>
        public Connection()
        {
            OpenTrades = new List<Trade>();
        }
    }
}
