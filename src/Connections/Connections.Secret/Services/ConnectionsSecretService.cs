//----------------------------------------------------------------------------------------
// <copyright file="ConnectionsSecretService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

namespace ForexMiner.Heimdallr.Connections.Secret.Services
{
    /// <summary>
    /// Implementation for connection secret service
    /// </summary>
    public class ConnectionsSecretService : IConnectionsSecretService
    {
        /// <summary>
        /// KeyVault secret client
        /// </summary>
        private readonly SecretClient _secretClient;

        /// <summary>
        /// Connections secret service constructor
        /// Sets up the required dependencies
        /// </summary>
        /// <param name="secretClient">The KeyVault secret client</param>
        public ConnectionsSecretService(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        /// <summary>
        /// Get the secret to a connection ID
        /// </summary>
        /// <param name="connectionId">The id of the connection</param>
        /// <returns>The secret</returns>
        public async Task<string> GetConnectionSecret(Guid connectionId)
        {
            var secret = await _secretClient.GetSecretAsync(GetConnectionSecretName(connectionId));
            return secret.Value.Value;
        }

        /// <summary>
        /// Create a new secret to a connection ID
        /// </summary>
        /// <param name="connectionId">The ID of the connection</param>
        /// <param name="secretValue">The value of the secret</param>
        /// <returns>An async task</returns>
        public async Task CreateConnectionSecret(Guid connectionId, string secretValue)
        {
            await _secretClient.SetSecretAsync(GetConnectionSecretName(connectionId), secretValue);
        }

        /// <summary>
        /// Construct the name of the connection secret
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        private string GetConnectionSecretName(Guid connectionId) => $"ConnectionSecret-{connectionId}";
    }
}
