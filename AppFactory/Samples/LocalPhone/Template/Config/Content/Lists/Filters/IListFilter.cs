using Template.Data;

namespace Template.Config.Content.Lists.Filters
{
    public interface IListFilter
    {
        bool IncludeInList(ISocialActivityWrapper activity);
    }
}
