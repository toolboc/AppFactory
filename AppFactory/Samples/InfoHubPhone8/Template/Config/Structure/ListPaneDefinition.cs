using System.Windows.Markup;

namespace InfoHubPhone8.Config.Structure
{
    [ContentProperty("List")]
    public class ListPaneDefinition : PaneDefinition
    {
        public string ListName { get; set; }
        public string TemplateName { get; set; }
    }
}