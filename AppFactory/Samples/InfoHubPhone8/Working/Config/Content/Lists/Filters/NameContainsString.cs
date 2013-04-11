using System.Windows.Markup;
using InfoHubPhone8.Data;

namespace InfoHubPhone8.Config.Content.Lists.Filters
{
    [ContentProperty("SearchString")]
    public class NameContainsString : BaseListFilter
    {
        public string SearchString { get; set; }

        public override bool IncludeInList(ISocialActivityWrapper activity)
        {
            if (!base.IncludeInList(activity)) return false;

            return activity.FeedSource.SourceName.Contains(SearchString);
        }
    }
}