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

        public DevelopmentConnectionsSecretService(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public async Task CreateConnectionSecret(Guid connectionId, string secretValue)
        {
            // Instead of an actual keyvault use local in-memory and redis cache to store secrets
            await _cachingService.GetOrCreateValueAsync(GetConnectionSecretName(connectionId), () => secretValue);
        }

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
