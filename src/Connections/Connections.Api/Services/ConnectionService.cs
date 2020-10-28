//----------------------------------------------------------------------------------------
// <copyright file="ConnectionService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Api.Services
{
    using Contracts = Heimdallr.Common.Data.Contracts;
    using Database = Heimdallr.Common.Data.Database.Models;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using AutoMapper;
    using GeriRemenyi.Oanda.V20.Sdk;
    using GeriRemenyi.Oanda.V20.Sdk.Utilities;
    using System.Threading.Tasks;
    using ForexMiner.Heimdallr.Connections.Secret.Services;
    using ForexMiner.Heimdallr.Common.Data.Database.Models.User;

    /// <summary>
    /// Connection service implementation
    /// </summary>
    public class ConnectionService : IConnectionService
    {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly ForexMinerHeimdallrDbContext _dbContext;

        /// <summary>
        /// Connection secret service
        /// </summary>
        private readonly IConnectionsSecretService _connectionsSecretService;

        /// <summary>
        /// Auto mapper
        /// </summary>
        private readonly IMapper _mapper;

        public ConnectionService(ForexMinerHeimdallrDbContext dbContext, IConnectionsSecretService connectionsSecretService, IMapper mapper)
        {
            _dbContext = dbContext;
            _connectionsSecretService = connectionsSecretService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all connections for a specific user
        /// </summary>
        /// <param name="userId">Id of the user to get the connection for</param>
        /// <returns>An enumerable collection of the connections</returns>
        public async Task<IEnumerable<Contracts.Connection.Connection>> GetConnectionsForUser(Guid userId)
        {
            // Collect connections
            var connectionsInDb = GetConnectionsForUserFromDb(userId);
            var connections = _mapper.Map<IEnumerable<Contracts.Connection.Connection>>(connectionsInDb);

            // Collect open trades for them
            foreach (var conn in connections)
            {
                // Oanda
                if (conn.Broker == Database.Connection.Broker.Oanda)
                {
                    var oandaServer = conn.Type == Contracts.Connection.ConnectionType.Demo ? OandaServer.FxPractice : OandaServer.FxTrade;
                    var oandaConnection = new ApiConnection(oandaServer, await _connectionsSecretService.GetConnectionSecret(conn.Id));
                    var oandAccount = oandaConnection.GetAccount(conn.ExternalAccountId);
                    var oandaAccountDetails = await oandAccount.GetDetails();
                    conn.Balance = oandaAccountDetails.Balance;
                    conn.ProfitLoss = oandaAccountDetails.Pl;
                    conn.OpenTrades = _mapper.Map<IEnumerable<Contracts.Trade.Trade>>(await oandAccount.GetOpenTrades());
                }
            }

            // Return connections enhanced with open trades
            return connections;
        }

        /// <summary>
        /// Test a connection
        /// </summary>
        /// <param name="connectionTest">The connection to test</param>
        /// <returns>The connection test results</returns>
        public async Task<Contracts.Connection.ConnectionTestResults> TestConnection(Contracts.Connection.ConnectionTest connectionToTest)
        {
            // Oanda
            if (connectionToTest.Broker == Database.Connection.Broker.Oanda)
            {
                try
                {
                    // Try demo first
                    ApiConnection oandaConnection = new ApiConnection(OandaServer.FxPractice, connectionToTest.Secret);
                    var accounts = await oandaConnection.GetAccounts();
                    var accountIds = accounts.Select(account => account.Id);

                    return new Contracts.Connection.ConnectionTestResults()
                    {
                        Type = Contracts.Connection.ConnectionType.Demo,
                        AccountIds = accountIds
                    };
                }
                catch
                {
                    // If demo connection fails -> try live one
                    ApiConnection oandaConnection = new ApiConnection(OandaServer.FxTrade, connectionToTest.Secret);
                    var accounts = await oandaConnection.GetAccounts();
                    var accountIds = accounts.Select(account => account.Id);

                    return new Contracts.Connection.ConnectionTestResults()
                    {
                        Type = Contracts.Connection.ConnectionType.Demo,
                        AccountIds = accountIds
                    };
                }
            }

            // TODO: better exception
            throw new Exception("Connection test failed");
        }

        /// <summary>
        /// Create a new connection for the user
        /// </summary>
        /// <param name="userid">Id of the user to create the connection to</param>
        /// <param name="connectionToCreate">The connection object to create</param>
        /// <returns>The created connection</returns>
        public async Task<Contracts.Connection.Connection> CreateConnectionForUser(Guid userid, Contracts.Connection.ConnectionCreation connectionToCreate)
        {
            // First make sure that the connection is ok
            var connectionTest = _mapper.Map<Contracts.Connection.ConnectionTest>(connectionToCreate);
            var testResult = await TestConnection(connectionTest);

            // TODO: make sure selected account id is ok

            // If it is then
            // 1. Save the connection to the DB
            var connectionToDb = _mapper.Map<Database.Connection.Connection>(connectionToCreate);
            connectionToDb.Type = testResult.Type;
            connectionToDb.Owner = GetUserFromDb(userid);
            _dbContext.Add(connectionToDb);
            _dbContext.SaveChanges();

            // 2. Save to token to keyvault
            await _connectionsSecretService.CreateConnectionSecret(connectionToDb.Id, connectionToCreate.Secret);

            // Return created connection
            return _mapper.Map<Contracts.Connection.Connection>(connectionToDb);
        }


        /// <summary>
        /// Get all trade signals
        /// </summary>
        /// <returns>List of trade signals</returns>
        public IEnumerable<Contracts.Trade.TradeSignal> GetTradeSignals()
        {
            return _mapper.Map<IEnumerable<Contracts.Trade.TradeSignal>>(_dbContext.TradeSignals);
        }

        /// <summary>
        /// Get a user by it's user id
        /// </summary>
        /// <param name="userId">Id of the user to get</param>
        /// <returns>The user from the DB</returns>
        private User GetUserFromDb(Guid userId)
        {
            return _dbContext.Users.Where(user => user.Id == userId).SingleOrDefault();
        }

        /// <summary>
        /// Get connections from the DB for a specific user
        /// </summary>
        /// <param name="userId">Id of the user to get the connections to</param>
        /// <returns>Collection of connections for the user</returns>
        private IEnumerable<Database.Connection.Connection> GetConnectionsForUserFromDb(Guid userId)
        {
            return _dbContext.Connections
                .Include(connection => connection.Owner)
                .Where(connection => connection.Owner.Id == userId);
        }
    }
}
