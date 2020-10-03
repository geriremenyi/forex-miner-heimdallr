namespace ForexMiner.Heimdallr.Instruments.Worker
{
    using ForexMiner.Heimdallr.Instruments.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
                    IConfiguration configuration = hostContext.Configuration;
                    services.AddInstrumentStorageService(configuration["StorageAccount:Url"]);
                    services.AddHostedService<Worker>();
                });
    }
}
