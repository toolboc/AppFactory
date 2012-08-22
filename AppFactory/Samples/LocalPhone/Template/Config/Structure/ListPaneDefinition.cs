using System.Windows.Markup;

namespace Template.Config.Structure
{
    [ContentProperty("List")]
    public class ListPaneDefinition : PaneDefinition
    {
        public string ListName { get; set; }
        public string TemplateName { get; set; }
    }
}