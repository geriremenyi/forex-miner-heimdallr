//----------------------------------------------------------------------------------------
// <copyright file="InstrumentsStorageServiceConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Configuration
{
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Instrument storage configuration extension methods
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class InstrumentsStorageServiceConfigurationExtensions
    {
        /// <summary>
        /// Adding azure storage account and local storage service to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="storageUrl">Azure storage account URL</param>
        public static void AddInstrumentsStorageServices(this IServiceCollection services, string storageUrl)
        {
            // Storage account
            services.AddAzureClients(builder => builder.AddBlobServiceClient(storageUrl));

            // Instrument service
            services.AddScoped<IInstrumentStorageService, InstrumentStorageService>();
        }
    }
}
