using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InfoHub.Articles
{
    public class TwitterArticle : ArticleBase
    {
        public override void MapProperties(SyndicationItem item, SyndicationFeed feed)
        {
            base.MapProperties(item, feed);

            try
            {
                if (item.Authors.Any())
                {
                    this.Author = item.Authors.First().NodeValue
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty);
                    this.Author = Regex.Replace(this.Author, @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)", string.Empty).Trim();
                }

                var _Tweet = item.Title.Text;
                _Tweet = Regex.Replace(_Tweet, @"^\w+:", string.Empty);
                _Tweet = Regex.Replace(_Tweet, "<[^>]*>", "LINK");
                var _Array = _Tweet.Split(' ');
                foreach (var _String in _Array)
                {
                    if (_String.Length == 1
                        || _String.StartsWith("#")
                        || _String.StartsWith("@")
                        || _String.StartsWith("http")
                        || _String.StartsWith("RT"))
                        _Tweet = _Tweet.Replace(_String, string.Empty);
                    _Tweet = _Tweet.Replace("  ", " ");
                }
                _Tweet = _Tweet.Replace("...", string.Empty);
                _Tweet = _Tweet.Replace(":", string.Empty);
                _Tweet = _Tweet.Replace("  ", " ");
                this.Body = _Tweet.Trim();
            }
            catch { System.Diagnostics.Debugger.Break(); }
        }
    }
}
