using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;

namespace NewsTextScraper.Api.Services
{
    public class FeedReader : IFeedReader
    {
        public async Task<List<string>> GetFeedLinksAsync(string feedUrl)
        {
            var items = new List<ISyndicationItem>();
            using (var xmlReader = XmlReader.Create(feedUrl, new XmlReaderSettings { Async = true }))
            {
                var feedReader = new RssFeedReader(xmlReader);

                while (await feedReader.Read())
                {
                    switch (feedReader.ElementType)
                    {
                        case SyndicationElementType.Item:
                            items.Add(await feedReader.ReadItem());
                            break;
                    }
                }
            }
            return items.Select(i => i.Links.FirstOrDefault()?.Uri.ToString()).ToList();
        }
    }
}
