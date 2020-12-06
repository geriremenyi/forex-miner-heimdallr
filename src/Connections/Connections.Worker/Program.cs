//----------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace Connections.Worker
{
    using ForexMiner.Heimdallr.Common.Extensions.Configuration;
    using ForexMiner.Heimdallr.Connections.Worker.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Connections worker program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program entry point for connections worker
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
                    services.AddConnectionsWorkerServices(hostContext.HostingEnvironment, hostContext.Configuration);
                    services.AddHostedService<Worker>();
                });
    }
}
