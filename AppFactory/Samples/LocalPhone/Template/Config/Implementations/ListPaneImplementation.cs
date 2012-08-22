using System.Linq;
using Template.Config.Content.Lists;
using Template.Config.Structure;

namespace Template.Config.Implementations
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