//----------------------------------------------------------------------------------------
// <copyright file="Worker.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace Connections.Worker
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using ForexMiner.Heimdallr.Connections.Worker.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
        /// Connections worker timer
        /// </summary>
        private Timer _timer;


        public Worker(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        /// <summary>
        /// Start connections worker
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A complete task on start</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var utcNow = DateTime.UtcNow;
            var oneMinuteLater = utcNow.AddMinutes(1);
            var nextMinuteAtFifthSecond = new DateTime(oneMinuteLater.Year, oneMinuteLater.Month, oneMinuteLater.Day, oneMinuteLater.Hour, oneMinuteLater.Minute, 5, DateTimeKind.Utc);
            _timer = new Timer(
                Tick,
                null,
                TimeSpan.FromSeconds((nextMinuteAtFifthSecond - utcNow).TotalSeconds),
                TimeSpan.FromMinutes(int.Parse(_configuration["ConnectionsWorker-IntervalMin"]))
            );
            return Task.CompletedTask;
        }

        /// <summary>
        /// Process ticks every minute
        /// </summary>
        /// <param name="source">The starter object</param>
        private async void Tick(Object source)
        {
            using var scope = _scopeFactory.CreateScope();
            var tickerService = scope.ServiceProvider.GetRequiredService<ITickerService>();
            await tickerService.Tick();
        }

        /// <summary>
        /// Stop connections worker processing
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A completed task</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Dispose connections worker and it's timer
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
