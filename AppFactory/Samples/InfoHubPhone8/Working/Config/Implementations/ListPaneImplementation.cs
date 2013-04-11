using System.Linq;
using InfoHubPhone8.Config.Content.Lists;
using InfoHubPhone8.Config.Structure;

namespace InfoHubPhone8.Config.Implementations
{
    public class ListPaneImplementation : PaneImplementation
    {
        public FilteredActivityList List { get; set; }

        public ListPaneImplementation(IRepository repository, ListPaneDefinition paneDefinition)
            : base(repository, paneDefinition)
        {
            var def = paneDefinition;
            List = (from list in Repository.FilteredLists
                    where list.ListName == def.ListName
                    select list).FirstOrDefault();
        }
    }
}