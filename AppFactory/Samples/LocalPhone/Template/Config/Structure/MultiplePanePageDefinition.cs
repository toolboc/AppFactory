using System.Collections.Generic;
using System.Windows.Markup;

namespace Template.Config.Structure
{
    [ContentProperty("Panes")]
    public class MultiplePanePageDefinition : PageDefinition
    {
        public string Title { get; set; }

        public List<PaneDefinition> Panes { get; set; }

        public MultiplePanePageDefinition()
        {
            Panes = new List<PaneDefinition>();
        }
    }
}