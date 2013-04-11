using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoHub.Feeds
{
    public interface IFeed
    {
        string Image { get; }
        string MoreUrl { get; }
        string SourceUrl { get; }
        string Title { get; }
        DateTime Updated { get; }
        List<InfoHub.Articles.IArticle> Articles { get; set; }
        bool Busy { get; set; }

        Task<bool> ReadFromWeb();
        Task<bool> ReadFromCache();
    }
}
