//----------------------------------------------------------------------------------------
// <copyright file="DevelopmentConnectionsSecretService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Secret.Services
{
    using ForexMiner.Heimdallr.Common.Caching.Services;
    using System;
    using System.Threading.Tasks;

    public class DevelopmentConnectionsSecretService : IConnectionsSecretService
    {
        /// <summary>
        /// Caching service which will replace keyvault
        /// </summary>
        private readonly ICachingService _cachingService;

        /// <summary>
        /// Development connections secret service constructor
        /// Sets up the dependencies
        /// </summary>
        /// <param name="cachingService">The caching service as the secret storage</param>
        public DevelopmentConnectionsSecretService(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        /// <summary>
        /// Create a new secret to a connection ID
        /// </summary>
        /// <param name="connectionId">The ID of the connection</param>
        /// <param name="secretValue">The value of the secret</param>
        /// <returns>An async task</returns>
        public async Task CreateConnectionSecret(Guid connectionId, string secretValue)
        {
            // Instead of an actual keyvault use local in-memory and redis cache to store secrets
            await _cachingService.GetOrCreateValueAsync(GetConnectionSecretName(connectionId), () => secretValue);
        }

        /// <summary>
        /// Get the secret to a connection ID
        /// </summary>
        /// <param name="connectionId">The id of the connection</param>
        /// <returns>The secret</returns>
        public async Task<string> GetConnectionSecret(Guid connectionId)
        {
            // Note that locally a cache can easily expire if not queried the amount of time set in the caching service
            return await _cachingService.GetOrCreateValueAsync(GetConnectionSecretName(connectionId), () => "SecretIsNotPresent");
        }

        /// <summary>
        /// Construct the name of the connection secret
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        private string GetConnectionSecretName(Guid connectionId) => $"ConnectionSecret-{connectionId}";
    }
}
