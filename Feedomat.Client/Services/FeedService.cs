using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Feedomat.Client.Services
{
    public interface IFeedService
    {
        Task<SyndicationFeed> GetSyndicationFeed();
    }

    public class FeedService: IFeedService
    {
        private HttpClient httpClient;
        public FeedService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<SyndicationFeed> GetSyndicationFeed()
        {
            SyndicationFeed syndicationFeed = new();
            var response = await httpClient.GetAsync("https://www.dailymail.co.uk/home/index.rss");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                XDocument xdoc = XDocument.Parse(await response.Content.ReadAsStringAsync());

                StringReader sr = new StringReader(xdoc.ToString());

                var stream = RetrieveStreamFromString(await sr.ReadToEndAsync());

                syndicationFeed = SyndicationFeed.Load(XmlReader.Create(stream));
            }
            return syndicationFeed;
        }
        public Stream RetrieveStreamFromString(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}
