//----------------------------------------------------------------------------------------
// <copyright file="InstrumentsWorkerConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Worker.Configuration
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Mapping;
    using ForexMiner.Heimdallr.Common.Extensions;
    using ForexMiner.Heimdallr.Instruments.Configuration;
    using ForexMiner.Heimdallr.Instruments.Worker.Services;
    using GeriRemenyi.Oanda.V20.Sdk;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Configuration extensions for instruments worker
    /// </summary>
    public static class InstrumentsWorkerConfigurationExtensions
    {

        /// <summary>
        /// Add required services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration object</param>
        public static void AddInstrumentsWorkerServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Instrument storage services
            services.AddInstrumentsStorageServices(configuration["StorageAccount-ConnectionString"]);

            // Engine API HttpClientFactory
            services.AddHttpClient(configuration["forex-miner-thor:Name"], client =>
            {
                client.BaseAddress = new Uri(configuration["forex-miner-thor:BaseAddress"]);
                client.DefaultRequestHeaders.Add("Accept", configuration["forex-miner-thor:Accept"]);
            });

            // Database
            services.AddDatabase(configuration["SqlServer-ConnectionString"]);

            // Auto mapper
            services.AddAutoMapper(typeof(ContractContractMappings), typeof(DatabaseContractMappings), typeof(OandaContractMappings));

            // Oanda API connection factory
            services.AddScoped<IOandaApiConnectionFactory, OandaApiConnectionFactory>();

            // Instrument worker services
            services.AddScoped<IInstrumentHistoryService, InstrumentHistoryService>();
        }
    }
}
