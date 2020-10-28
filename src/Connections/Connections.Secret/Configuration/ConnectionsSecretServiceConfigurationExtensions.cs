//----------------------------------------------------------------------------------------
// <copyright file="ConnectionsSecretServiceConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Secret.Configuration
{
    using Azure.Identity;
    using ForexMiner.Heimdallr.Common.Extensions;
    using ForexMiner.Heimdallr.Connections.Secret.Services;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Configuration extensions for the connections secrets
    /// </summary>
    public static class ConnectionsSecretServiceConfigurationExtensions
    {

        /// <summary>
        /// Add required services for secret service
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="keyVaultUrl">The URI of the keyvault</param>
        public static void AddConnectionsSecretServices(this IServiceCollection services, bool isDevelopmentEnvironment, string keyVaultUri, string redisConnectionString)
        {
            if (isDevelopmentEnvironment)
            {
                // DEV 
                // workaround as there is no chance to run a local keyvaul emulator as there is no such thing
                services.AddCachingService(redisConnectionString);
                services.AddScoped<IConnectionsSecretService, DevelopmentConnectionsSecretService>();
            }
            else 
            {
                // PRODUCTION
                services.AddAzureClients(builder => {
                    builder.AddSecretClient(new Uri(keyVaultUri));
                    builder.UseCredential(new DefaultAzureCredential(true));
                });

                services.AddScoped<IConnectionsSecretService, ConnectionsSecretService>();
            }
        }

    }
}
