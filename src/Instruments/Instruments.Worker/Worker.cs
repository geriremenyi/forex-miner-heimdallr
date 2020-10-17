namespace ForexMiner.Heimdallr.Instruments.Worker
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using ForexMiner.Heimdallr.Instruments.Worker.Services.History;
    using Microsoft.Extensions.Hosting;

    public class Worker : BackgroundService
    {
        private readonly IHistoryService _historyService;

        public Worker(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var oneMinute = TimeSpan.FromMinutes(1);
            var cycles = 1;

            while (!stoppingToken.IsCancellationRequested)
            {
                // Starting a stopwatch
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Every hour check and fill data for 
                // all instruments until yesterday
                if (cycles == 1) 
                {
                    _ = _historyService.CheckAndFillUntilYesterday();
                    cycles = 1;
                }

                // Every minute check and fill data for today
                //_ = _historyService.CheckAndFillToday();

                // Stopping the stopwatch and waiting till 1 minute expires
                // including the latest execution time
                stopwatch.Stop();
                await Task.Delay(oneMinute.Add(-1 * stopwatch.Elapsed), stoppingToken);
            }
        }
    }
}
