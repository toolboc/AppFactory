using System.Collections.Generic;
using System.Windows.Markup;

namespace InfoHubPhone8.Config.Structure
{
    [ContentProperty("Links")]
    public class LinkPaneDefinition : PaneDefinition
    {
        public List<LinkDefinition> Links { get; set; }

        public LinkPaneDefinition()
        {
            Links = new List<LinkDefinition>();
        }
    }
}