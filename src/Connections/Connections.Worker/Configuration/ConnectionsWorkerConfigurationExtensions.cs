//----------------------------------------------------------------------------------------
// <copyright file="ConnectionsWorkerConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Worker.Configuration
{
    using ForexMiner.Heimdallr.Common.Extensions;
    using ForexMiner.Heimdallr.Connections.Secret.Configuration;
    using ForexMiner.Heimdallr.Connections.Worker.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Hosting;
    using System;
    using Microsoft.Extensions.Hosting;
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Mapping;

    /// <summary>
    /// Extension methods for connections worker service configuration
    /// </summary>
    public static class ConnectionsWorkerConfigurationExtensions
    {
        /// <summary>
        /// Add required services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration object</param>
        public static void AddConnectionsWorkerServices(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
        {
            // Engine API HttpClientFactory
            services.AddHttpClient(configuration["forex-miner-thor:Name"], client =>
            {
                client.BaseAddress = new Uri(configuration["forex-miner-thor:BaseAddress"]);
                client.DefaultRequestHeaders.Add("Accept", configuration["forex-miner-thor:Accept"]);
            });

            // Database
            services.AddDatabase(configuration["SqlServer-ConnectionString"]);

            // Auto mapping
            services.AddAutoMapper(typeof(ContractContractMappings), typeof(DatabaseContractMappings), typeof(OandaContractMappings));

            // Connections secret service
            services.AddConnectionsSecretServices(environment.IsDevelopment(), configuration["KeyVault-Uri"], configuration["RedisCache-ConnectionString"]);

            // Connections worker services
            services.AddScoped<ITickerService, TickerService>();
        }
    }
}
