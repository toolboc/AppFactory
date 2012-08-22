using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Template.Data;

namespace Template.Config.Content.Lists
{
    public class FilteredActivityList
    {
        private readonly ObservableCollection<ISocialActivityWrapper> activities = new ObservableCollection<ISocialActivityWrapper>();

        public IReaderList Definition { get; set; }

        public FilteredActivityList()
        {
            Filters = new List<Func<ISocialActivityWrapper, bool>>();
        }

        public FilteredActivityList(IReaderList filterDefinition)
            : this()
        {
            // Set the list name (ie the header on the panorama item)
            ListName = filterDefinition.ListName;
            Title = filterDefinition.Title;
            ShortTitle = filterDefinition.ShortTitle;
            Definition = filterDefinition;
            // Determine the filter rule to determine which items go in this list
            Filters.AddRange(from f in filterDefinition.Filters
                             select (Func<ISocialActivityWrapper, bool>)f.IncludeInList);
        }

        public ObservableCollection<ISocialActivityWrapper> Activities
        {
            get { return activities; }
        }

        public List<Func<ISocialActivityWrapper, bool>> Filters { get; set; }

        public string ListName { get; set; }
        public string ShortTitle { get; set; }
        public string Title { get; set; }

        public virtual bool IncludeInList(ISocialActivityWrapper activity)
        {
            return Filters.All(listFilter => listFilter(activity));
        }
    }
}