using Template.Data;

namespace Template.Config.Content.Lists.Filters
{
    public class VideoFilter : BaseListFilter
    {
        public override bool IncludeInList(ISocialActivityWrapper activity)
        {
            return activity.VideosExist;
        }
    }
}
