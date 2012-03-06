using Template.Data;

namespace Template.Config.Content.Lists.Filters
{
    public class HasImageFilter : BaseListFilter
    {
        public override bool IncludeInList(ISocialActivityWrapper activity)
        {
            return activity.ImagesExist;
        }
    }
}
