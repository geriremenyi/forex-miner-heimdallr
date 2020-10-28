//----------------------------------------------------------------------------------------
// <copyright file="KeyVaultConfigurationExtensions.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Extensions.Configuration
{
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.AzureKeyVault;
    using Microsoft.Extensions.Hosting;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Extensions methods for keyvault configuration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class KeyVaultConfigurationExtensions
    {
        /// <summary>
        /// Configure keyvault as configuration provider
        /// </summary>
        /// <param name="host">The host builder</param>
        /// <returns>The enhanced host builder</returns>
        public static IHostBuilder ConfigureKeyVaultConfigurationProvider(this IHostBuilder host) 
        {
            return host.ConfigureAppConfiguration((context, config) =>
            {
                if (!context.HostingEnvironment.IsDevelopment())
                {
                    // If KeyVault-Uri is provided and the environment is not development
                    // then it adds the KeyVault as a configuration provider
                    var builtConfig = config.Build();
                    var keyVaultUri = builtConfig["KeyVault-Uri"];
                    if (!string.IsNullOrEmpty(keyVaultUri))
                    {
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                        config.AddAzureKeyVault(keyVaultUri, keyVaultClient, new DefaultKeyVaultSecretManager());
                    }
                }
            });
        }

    }
}
