using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InfoHub.Articles
{
    public interface IArticle
    {
        string Author { get; set; }
        string Body { get; set; }
        DateTime Date { get; set; }
        string Image { get; set; }
        string Title { get; set; }
        string Url { get; set; }
        void MapProperties(IArticle article);
        void MapProperties(SyndicationItem item, SyndicationFeed feed);

        // layout oriented properties
        int Index { get; set; }
        bool Hero { get;  }
        int ColSpan { get; set; }
        int RowSpan { get; set; }
    }
}
