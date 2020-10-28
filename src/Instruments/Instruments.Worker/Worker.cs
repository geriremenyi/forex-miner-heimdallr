//----------------------------------------------------------------------------------------
// <copyright file="Worker.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Worker
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using ForexMiner.Heimdallr.Instruments.Worker.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Instruments worker
    /// </summary>
    public class Worker : IHostedService, IDisposable
    {
        /// <summary>
        /// Scope factory
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;


        /// <summary>
        /// Configuration object
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Instruments worker timer
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Instruments worker constructor
        /// Sets up the history service and configuration
        /// </summary>
        /// <param name="historyService"></param>
        public Worker(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        /// <summary>
        /// Start instruments worker
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A complete task on start</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                CheckInstrumentGranularitiesAndLoadData, 
                null, 
                TimeSpan.FromMinutes(int.Parse(_configuration["InstrumentsWorker-IntervalMin"])),
                TimeSpan.FromMinutes(int.Parse(_configuration["InstrumentsWorker-IntervalMin"]))
            );
            return Task.CompletedTask;
        }

        /// <summary>
        /// Process newly addedd instrument granularities and it's data
        /// </summary>
        /// <param name="source">The starter object</param>
        private async void CheckInstrumentGranularitiesAndLoadData(Object source)
        {
            using var scope = _scopeFactory.CreateScope();
            var historyServive = scope.ServiceProvider.GetRequiredService<IInstrumentHistoryService>();
             await historyServive.CheckInstrumentGranularitiesAndLoadData();
        }

        /// <summary>
        /// Stop instruments worker processing
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A completed task</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Dispose instruments worker and it's timer
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
