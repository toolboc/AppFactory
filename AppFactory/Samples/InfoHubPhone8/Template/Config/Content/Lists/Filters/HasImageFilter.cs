using InfoHubPhone8.Data;

namespace InfoHubPhone8.Config.Content.Lists.Filters
{
    public class HasImageFilter : BaseListFilter
    {
        public override bool IncludeInList(ISocialActivityWrapper activity)
        {
            return activity.ImagesExist;
        }
    }
}
