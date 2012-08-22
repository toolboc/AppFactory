using System.Windows.Markup;

namespace Template.Config.Structure
{
    [ContentProperty("Pane")]
    public class SinglePanePageDefinition : PageDefinition
    {
        public string Title { get; set; }

        public string SubTitle { get; set; }

        public PaneDefinition Pane { get; set; }
    }
}