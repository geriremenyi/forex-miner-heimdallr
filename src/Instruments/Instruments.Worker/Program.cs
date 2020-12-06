//----------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Worker
{
    using ForexMiner.Heimdallr.Common.Extensions.Configuration;
    using ForexMiner.Heimdallr.Instruments.Worker.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Instruments worker program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program entry point for instrument worker
        /// </summary>
        /// <param name="args">Arguments</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Create host builder
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>Host builder object</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureKeyVaultConfigurationProvider()
                .ConfigureSecretConfigurationConfigurationProvider()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddInstrumentsWorkerServices(hostContext.Configuration);
                    services.AddHostedService<Worker>();
                });
    }
}
