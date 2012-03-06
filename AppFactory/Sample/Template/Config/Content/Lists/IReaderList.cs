using System.Collections.Generic;
using Template.Config.Content.Lists.Filters;

namespace Template.Config.Content.Lists
{
    public interface IReaderList
    {
        string Title { get; set; }
        string ShortTitle { get; set; }
        string ListName { get; set; }

        List<IListFilter> Filters { get; }
    }
}