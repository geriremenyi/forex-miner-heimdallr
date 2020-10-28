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

    /// <summary>
    /// Representation of a connections
    /// </summary>
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
        /// Balance of the connection
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// Profit or loss of the connection
        /// Negative means loss, posistive means profit
        /// </summary>
        public double ProfitLoss { get; set; }

        /// <summary>
        /// Trades made through the connection
        /// </summary>
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
