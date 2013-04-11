using InfoHubPhone8.Data;

namespace InfoHubPhone8.Config.Content.Lists.Filters
{
    public class VideoFilter : BaseListFilter
    {
        public override bool IncludeInList(ISocialActivityWrapper activity)
        {
            return activity.VideosExist;
        }
    }
}
