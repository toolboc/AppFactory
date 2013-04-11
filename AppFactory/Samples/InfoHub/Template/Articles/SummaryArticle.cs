using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InfoHub.Articles
{
    public class SummaryArticle: ArticleBase
    {
        public override void MapProperties(SyndicationItem item, SyndicationFeed feed)
        {
            base.MapProperties(item, feed);
        }

        object m_Data = default(object);
        public object Data { get { return m_Data; } set { SetProperty(ref m_Data, value); } }
    }
}
