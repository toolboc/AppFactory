using InfoHub.Articles;
using InfoHub.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InfoHub.Feeds
{
    public class RssFeed<T> : FeedBase where T : IArticle, new()
    {
        public override async Task<bool> ReadFromWeb()
        {
            if (this.Busy)
                return false;
            this.Busy = true;

            try
            {
                // fetch the feed 
                SyndicationFeed _Feed = null;
                try { _Feed = await ReadFromWebWithSyndication(); }
                catch
                {
                    Debug.WriteLine("Error reading: " + this.SourceUrl);
                    System.Diagnostics.Debugger.Break();
                }
                if (_Feed == null)
                    try { _Feed = await ReadFromWebWithAlternative(); }
                    catch
                    {
                        Debug.WriteLine("Error reading: " + this.SourceUrl);
                        System.Diagnostics.Debugger.Break();
                    }
                if (_Feed == null)
                    return false;

                // map in feed properties
                this.Title = this.Title ?? _Feed.Title.Text;

                // merge items from feed
                var _List = new List<T>();
                foreach (var item in _Feed.Items)
                {
                    var _Article = new T();
                    _Article.MapProperties(item, _Feed);
                    _List.Add(_Article);
                }
                base.MergeArticles(_List as IEnumerable<IArticle>);
                this.Updated = DateTime.Now;
                await base.WriteToCache();
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                System.Diagnostics.Debugger.Break();
                return false;
            }
            finally
            {
                this.Busy = false;
            }
        }

        async Task<SyndicationFeed> ReadFromWebWithSyndication()
        {
            // fetch latest from the internet
            var _Uri = new Uri(this.SourceUrl);
            var _Feed = await new SyndicationClient().RetrieveFeedAsync(_Uri);
            return _Feed;
        }

        async Task<SyndicationFeed> ReadFromWebWithAlternative()
        {
            // including user agent, otherwise FB rejects the request
            var _Client = new HttpClient();
            var _UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            _Client.DefaultRequestHeaders.Add("user-agent", _UserAgent);

            // fetch as string to avoid error
            var _Uri = new Uri(this.SourceUrl);
            var _Response = await _Client.GetAsync(_Uri);
            var _String = await _Response.Content.ReadAsStringAsync();

            // convert to xml (will validate, too)
            var _XmlDocument = new Windows.Data.Xml.Dom.XmlDocument();
            _XmlDocument.LoadXml(_String);

            // manually fill feed from xml
            var _Feed = new Windows.Web.Syndication.SyndicationFeed();
            _Feed.LoadFromXml(_XmlDocument);
            return _Feed;
        }
    }
}
