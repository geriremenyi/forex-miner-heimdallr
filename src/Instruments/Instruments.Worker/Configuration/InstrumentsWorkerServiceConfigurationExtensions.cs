using ForexMiner.Heimdallr.Instruments.Configuration;
using ForexMiner.Heimdallr.Instruments.Worker.Services.History;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForexMiner.Heimdallr.Instruments.Worker.Configuration
{
    public static class InstrumentsWorkerServiceConfigurationExtensions
    {

        public static void AddInstrumentsWorkerServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Instrument storage services
            services.AddInstrumentsStorageServices(configuration["StorageAccount:Url"]);

            // Instrument worker HttpClientFactories
            services.AddHttpClient(configuration["ForexMiner.Heimdallr.Instruments.Api:Name"], client =>
            {
                client.BaseAddress = new Uri(configuration["ForexMiner.Heimdallr.Instruments.Api:BaseAddress"]);
                client.DefaultRequestHeaders.Add("Accept", configuration["ForexMiner.Heimdallr.Instruments.Api:Accept"]);
            });

            // Instrument worker services
            services.AddSingleton<IHistoryService, HistoryService>();
        }

    }
}
