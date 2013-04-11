using System.Collections.Generic;
using InfoHubPhone8.Config.Content.Lists.Filters;

namespace InfoHubPhone8.Config.Content.Lists
{
    public interface IReaderList
    {
        string Title { get; set; }
        string ShortTitle { get; set; }
        string ListName { get; set; }

        List<IListFilter> Filters { get; }
    }
}