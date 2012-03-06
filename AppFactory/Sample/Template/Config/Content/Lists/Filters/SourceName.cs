using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Template.Data;

namespace Template.Config.Content.Lists.Filters
{
    [ContentProperty("Names")]
    public class SourceName : BaseListFilter
    {
        public SourceName()
        {
            Names = new List<string>();
        }

        public string Name
        {
            get
            {
                if (Names != null && Names.Count > 0) return Names[0];
                return null;
            }
            set
            {
                if (Names == null) return;
                Names.Clear();
                Names.Add(value);
            }
        }


        public List<string> Names { get; set; }

        public override bool IncludeInList(ISocialActivityWrapper activity)
        {
            if (!base.IncludeInList(activity)) return false;

            //if (!string.IsNullOrEmpty(Name))
            //{
            //    return activity.FeedSource.Name == Name;
            //}

            if (Names != null && Names.Count > 0)
            {
                return Names.Any(source => activity.FeedSource.SourceName == source);
            }
            return false;
        }
    }
}
