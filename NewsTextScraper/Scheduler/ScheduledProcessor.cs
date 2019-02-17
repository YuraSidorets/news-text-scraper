using Microsoft.Extensions.DependencyInjection;
using NCrontab;
using NewsTextScraper.BackgroundService;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NewsTextScraper.Scheduler
{
    public abstract class ScheduledProcessor : ScopedProcessor
    {
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly ILogger _logger;
        protected abstract string Schedule { get; }

        protected ScheduledProcessor(IServiceScopeFactory serviceScopeFactory, ILogger<ScheduledProcessor> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            _schedule = CrontabSchedule.Parse(Schedule);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                var nextrun = _schedule.GetNextOccurrence(now);
                if (now > _nextRun)
                {
                    try
                    {
                        _logger.LogInformation("Scheduled process started");
                        await Process();
                        _logger.LogInformation("Scheduled process ended");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Scheduler processing error occured.");
                    }
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(5000, stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }
    }
}
