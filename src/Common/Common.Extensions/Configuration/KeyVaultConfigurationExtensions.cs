namespace ForexMiner.Heimdallr.Common.Extensions.Configuration
{
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.AzureKeyVault;
    using Microsoft.Extensions.Hosting;

    public static class KeyVaultConfigurationExtensions
    {
        public static IHostBuilder ConfigureKeyVaultConfigurationProvider(this IHostBuilder host) 
        {
            return host.ConfigureAppConfiguration((context, config) =>
            {
                if (!context.HostingEnvironment.IsDevelopment())
                {
                    // If KeyVault:Uri is provided and the environment is not development
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
