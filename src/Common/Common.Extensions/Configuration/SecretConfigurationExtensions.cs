//----------------------------------------------------------------------------------------
// <copyright file="SecretConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Extensions.Configuration
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Extensions methods for keyvault configuration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SecretConfigurationExtensions
    {
        /// <summary>
        /// Configure keyvault as configuration provider
        /// </summary>
        /// <param name="host">The host builder</param>
        /// <returns>The enhanced host builder</returns>
        public static IHostBuilder ConfigureSecretConfigurationConfigurationProvider(this IHostBuilder host) 
        {
            return host.ConfigureAppConfiguration((context, config) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    // Add the secret file if environment is development
                    config.AddJsonFile("appsettings.Secrets.json", true, true);
                }
            });
        }

    }
}
