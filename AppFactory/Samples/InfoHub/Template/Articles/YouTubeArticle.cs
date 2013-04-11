using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InfoHub.Articles
{
    public class YouTubeArticle : ArticleBase
    {
        public override void MapProperties(SyndicationItem item, SyndicationFeed feed)
        {
            base.MapProperties(item, feed);

            try
            { // parse out image source URL
                const string _Pattern = "<img.+?src=[\"'](.+?)[\"'].+?>";
                var _Match = Regex.Match(this.Body, _Pattern, RegexOptions.IgnoreCase);
                var _Group = _Match.Groups[1];
                var _Value = _Group.Value;
                this.Image = _Value;

                this.Url = item.Links.First().Uri.ToString();
            }
            catch { System.Diagnostics.Debugger.Break(); }
        }
    }
}
