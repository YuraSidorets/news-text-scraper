using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NewsTextScraper.Api.Models;

namespace NewsTextScraper.Api.Services
{
    public class ScraperService : IScraperService
    {
        private readonly ILogger _logger;

        public ScraperService(ILogger<ScraperService> logger)
        {
            _logger = logger;
        }

        public async Task<WebPageData> ScrapWebPageAsync(string url)
        {
            var content = await GetPageContentAsync(url);
            var title = GetPageTitle(content);
            var text = ParsePageContent(content);
            var webPageData = new WebPageData(url: url, title: title, content: text);
            _logger.LogInformation($"Created web page data: \n Id: {webPageData.Id}\n Url: {url} \n Title: {title} \n Content-Length: {text.Length}");

            return webPageData;
        }

        public async Task<string> ScrapWebPageTextAsync(string url)
        {
            return ParsePageContent(await GetPageContentAsync(url));
        }

        private string ParsePageContent(HtmlNode html)
        {
            CleanScriptsAndStyles(html);
            var divTags = html.CssSelect("div").ToList();
            return string.Join(" ", divTags.Select(x => HttpUtility.HtmlDecode(x.InnerText.CleanInnerText())).Distinct());
        }

        private async Task<HtmlNode> GetPageContentAsync(string url)
        {
            var browser = new ScrapingBrowser();
            var page = await browser.NavigateToPageAsync(new Uri(url));
            return page.Html;
        }

        private string GetPageTitle(HtmlNode html)
        {
            return html.CssSelect("title").FirstOrDefault()?.InnerText ?? string.Empty;
        }

        private void CleanScriptsAndStyles(HtmlNode html)
        {
            html.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style")
                .ToList()
                .ForEach(n => n.Remove());
        }
    }
}
