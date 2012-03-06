using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Template.Config.Content.Lists
{
    [ContentProperty("ReaderLists")]
    public class DefaultListCreator : IListCreator
    {
        public DefaultListCreator()
        {
            ReaderLists = new List<IReaderList>();
        }

        public List<IReaderList> ReaderLists { get; set; }


        public void CreateActivityLists(IRepository repository)
        {
            if (repository == null) return;
            List<FilteredActivityList> activityFilters = repository.FilteredLists;

            if (ReaderLists == null) return;

            activityFilters.AddRange(from listDefinition in ReaderLists
                                     select CreateReaderList(listDefinition));
        }

        public virtual FilteredActivityList CreateReaderList(IReaderList listDefinition)
        {
            return new FilteredActivityList(listDefinition);
        }
    }
}
