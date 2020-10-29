//----------------------------------------------------------------------------------------
// <copyright file="IConnectionService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Api.Services
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Connection;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Trade;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Connection service interface
    /// </summary>
    public interface IConnectionService
    {
        /// <summary>
        /// Get all connections for a specific user
        /// </summary>
        /// <param name="userId">Id of the user to get the connection for</param>
        /// <returns>An enumerable collection of the connections</returns>
        public Task<IEnumerable<Connection>> GetConnectionsForUser(Guid userId);

        /// <summary>
        /// Test a connection
        /// </summary>
        /// <param name="connectionTest">The connection to test</param>
        /// <returns>The connection test results</returns>
        public ConnectionTestResults TestConnection(ConnectionTest connectionTest);

        /// <summary>
        /// Create a new connection for the user
        /// </summary>
        /// <param name="userid">Id of the user to create the connection to</param>
        /// <param name="connection">The connection object to create</param>
        /// <returns>The created connection</returns>
        public Task<Connection> CreateConnectionForUser(Guid userid, ConnectionCreation connection);

        /// <summary>
        /// Get all trade signals
        /// </summary>
        /// <returns>List of trade signals</returns>
        public IEnumerable<TradeSignal> GetTradeSignals();
    }
}
