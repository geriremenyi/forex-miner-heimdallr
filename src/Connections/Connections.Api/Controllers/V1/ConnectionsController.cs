//----------------------------------------------------------------------------------------
// <copyright file="ConnectionsController.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Api.Controllers.V1
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.Connection;
    using ForexMiner.Heimdallr.Common.Data.Contracts.Trade;
    using ForexMiner.Heimdallr.Connections.Api.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// Connections controller
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/users/me/connections")]
    public class ConnectionsController : ControllerBase
    {
        /// <summary>
        /// Connection service
        /// </summary>
        private readonly IConnectionService _connectionService;

        /// <summary>
        /// Connection controller constructor
        /// Sets up the required services
        /// </summary>
        /// <param name="connectionService"></param>
        public ConnectionsController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        /// <summary>
        /// Get connections endpoint
        /// </summary>
        /// <returns>All connections for the user</returns>
        [HttpGet]
        public async Task<IEnumerable<Connection>> GetConnectionsForUser()
        {
            return await _connectionService.GetConnectionsForUser(WhoAmI());
        }

        /// <summary>
        /// Test a connection for a user
        /// Not bound to the user but hidden behind a login so it is not 
        /// open for everyone, only registered users can access this
        /// </summary>
        /// <returns>The connection test results</returns>
        [HttpPost("test")]
        public async Task<ConnectionTestResults> TestConnection([FromBody] ConnectionTest connectionTest)
        {
            return await _connectionService.TestConnection(connectionTest);
        }

        /// <summary>
        /// Create a new connection for an user
        /// </summary>
        /// <param name="connection">The connection to create</param>
        /// <returns>The connection created</returns>
        [HttpPost]
        public async Task<Connection> CreateConnection([FromBody] ConnectionCreation connection)
        {
            return await _connectionService.CreateConnectionForUser(WhoAmI(), connection);
        }

        /// <summary>
        /// Get trade signals
        /// </summary>
        /// <returns></returns>
        [HttpGet("trade-signals")]
        public IEnumerable<TradeSignal> GetTradeSignals()
        {
            return _connectionService.GetTradeSignals();
        }

        /// <summary>
        /// Find out the id of the user sending the request
        /// </summary>
        /// <returns></returns>
        private Guid WhoAmI()
        {
            return Guid.Parse(User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value);
        }
    }
}
