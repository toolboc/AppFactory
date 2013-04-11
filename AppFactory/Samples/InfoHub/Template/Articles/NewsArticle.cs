using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InfoHub.Articles
{
    public class NewsArticle : ArticleBase
    {
        public override void MapProperties(SyndicationItem item, SyndicationFeed feed)
        {
            base.MapProperties(item, feed);

            // strip html from the body
            this.Body = Regex.Replace(this.Body, "<.*?>", string.Empty);

            if (!this.Url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                try { this.Url = item.Links.First().NodeValue; }
                catch { }
        }
    }
}
