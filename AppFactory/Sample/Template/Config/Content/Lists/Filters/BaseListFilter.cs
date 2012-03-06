using Template.Data;

namespace Template.Config.Content.Lists.Filters
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