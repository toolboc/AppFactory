using InfoHub.Articles;
using InfoHub.Feeds;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoHub.Helpers
{
    public class GroupedItem
    {
        public GroupedItem(IFeed feed)
        {
            Articles = new ObservableCollection<IArticle>();
            this.Feed = feed;
        }
        public IFeed Feed { get; set; }
        public string Header { get { return Feed.Title; } }
        public ObservableCollection<IArticle> Articles { get; set; }
    }
}
