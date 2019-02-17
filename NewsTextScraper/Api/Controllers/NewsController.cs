using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NewsTextScraper.Api.Models;
using NewsTextScraper.Api.Services;

namespace NewsTextScraper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IScraperService _scraperService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;


        public NewsController(IScraperService scraperService, ILogger<NewsController> logger, IMemoryCache cache)
        {
            _scraperService = scraperService;
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data =_cache.Get<List<WebPageData>>("WebPageData") ?? new List<WebPageData>();
            return new JsonResult(data);
        }
    }
}
