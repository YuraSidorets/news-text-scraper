using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsTextScraper.Api.Models;
using NewsTextScraper.Api.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsTextScraper.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {
        private readonly IMemoryCache _cache;
        private readonly IScraperService _scraperService;
        private readonly ILogger _logger;
        private readonly ILogger<ScheduledProcessor> _baseLogger;
        private readonly IOptions<FeedUrls> _feedConfiguration;
        private readonly IFeedReader _feedReader;
        protected override string Schedule => "*/1 * * * *";

        public ScheduleTask(
            IServiceScopeFactory serviceScopeFactory,
            IMemoryCache memoryCache,
            IScraperService scraperService,
            ILogger<ScheduleTask> logger,
            IOptions<FeedUrls> feedConfiguration,
            IFeedReader feedReader,
            ILogger<ScheduledProcessor> baseLogger) : base(serviceScopeFactory, baseLogger)
        {
            _cache = memoryCache;
            _scraperService = scraperService;
            _logger = logger;
            _feedConfiguration = feedConfiguration;
            _feedReader = feedReader;
            _baseLogger = baseLogger;
        }

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            var urls = new List<string>();
            foreach (var feedUrl in _feedConfiguration.Value.Urls)
            {
                new Func<Task>(async () => urls.AddRange(await _feedReader.GetFeedLinksAsync(feedUrl)))();
            }
            _logger.LogInformation($"{urls.Count} urls found");

            var webPagesData = new List<WebPageData>();
            foreach (var url in urls)
            {
                new Func<Task>(async () => webPagesData.Add(await _scraperService.ScrapWebPageAsync(url)))();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.NeverRemove)
                    .SetSlidingExpiration(TimeSpan.FromHours(2));

                _cache.Set("WebPageData", webPagesData, cacheEntryOptions);
            }
            return Task.CompletedTask;
        }
    }
}
