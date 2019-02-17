using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsTextScraper.Api.Services
{
    public interface IFeedReader
    {
        Task<List<string>> GetFeedLinksAsync(string feedUrl);
    }
}
