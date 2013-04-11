using InfoHubPhone8.Data;

namespace InfoHubPhone8.Config.Content.Lists.Filters
{
    public interface IListFilter
    {
        bool IncludeInList(ISocialActivityWrapper activity);
    }
}
