namespace ForexMiner.Heimdallr.Instruments.Configuration
{
    using ForexMiner.Heimdallr.Instruments.Storage.Services;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.DependencyInjection;

    public static class InstrumentStorageServiceConfigurationExtensions
    {
        public static void AddInstrumentStorageService(this IServiceCollection services, string storageUrl)
        {
            // Storage account
            services.AddAzureClients(builder => builder.AddBlobServiceClient(storageUrl));

            // Instrument service
            services.AddSingleton<IInstrumentStorageService, InstrumentStorageService>();
        }
    }
}
