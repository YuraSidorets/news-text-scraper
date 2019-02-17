using System.Threading.Tasks;
using NewsTextScraper.Api.Models;

namespace NewsTextScraper.Api.Services
{
    public interface IScraperService
    {
        Task<string> ScrapWebPageTextAsync(string url);

        Task<WebPageData> ScrapWebPageAsync(string url);
    }
}
