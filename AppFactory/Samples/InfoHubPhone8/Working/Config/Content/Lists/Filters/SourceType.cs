using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using InfoHubPhone8.Data;

namespace InfoHubPhone8.Config.Content.Lists.Filters
{
    [ContentProperty("Types")]
    public class SourceType : BaseListFilter
    {
        public SourceType()
        {
            Types = new List<string>();
        }

        public string Type
        {
            get
            {
                if (Types != null && Types.Count > 0) return Types[0];
                return null;
            }
            set
            {
                if (Types == null) return;
                Types.Clear();
                Types.Add(value);
            }
        }

        public List<string> Types { get; set; }

        public override bool IncludeInList(ISocialActivityWrapper activity)
        {
            if (!base.IncludeInList(activity)) return false;

            //if (!string.IsNullOrEmpty(Type))
            //{
            //    var src = Type;//.EnumParse<Source>();

            //    return activity.FeedSource.Source == src;
            //}

            if (Types != null && Types.Count > 0)
            {
                return Types.Any(src => activity.FeedSource.SourceTypeName == src);
            }
            return false;
        }
    }
}