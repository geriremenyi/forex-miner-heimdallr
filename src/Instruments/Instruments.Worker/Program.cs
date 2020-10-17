namespace ForexMiner.Heimdallr.Instruments.Worker
{
    using ForexMiner.Heimdallr.Instruments.Configuration;
    using ForexMiner.Heimdallr.Instruments.Worker.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddInstrumentsWorkerServices(hostContext.Configuration);
                    services.AddHostedService<Worker>();
                });
    }
}
