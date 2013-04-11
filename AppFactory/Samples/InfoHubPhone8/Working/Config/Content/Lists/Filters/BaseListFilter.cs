using InfoHubPhone8.Data;

namespace InfoHubPhone8.Config.Content.Lists.Filters
{
    public class BaseListFilter : IListFilter
    {
        public virtual bool IncludeInList(ISocialActivityWrapper activity)
        {
            if (activity == null || activity.SocialActivity == null) return false;
            return true;
        }
    }
}