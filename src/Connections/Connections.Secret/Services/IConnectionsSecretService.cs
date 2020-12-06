//----------------------------------------------------------------------------------------
// <copyright file="IConnectionsSecretService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Secret.Services
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for connection secret service
    /// </summary>
    public interface IConnectionsSecretService
    {
        /// <summary>
        /// Get the secret to a connection ID
        /// </summary>
        /// <param name="connectionId">The id of the connection</param>
        /// <returns>The secret</returns>
        public Task<string> GetConnectionSecret(Guid connectionId);

        /// <summary>
        /// Create a new secret to a connection ID
        /// </summary>
        /// <param name="connectionId">The ID of the connection</param>
        /// <param name="secretValue">The value of the secret</param>
        /// <returns>An async task</returns>
        public Task CreateConnectionSecret(Guid connectionId, string secretValue);
    }
}
