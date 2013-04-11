using InfoHubPhone8.Config.Content.Lists;
using InfoHubPhone8.Config.Structure;

namespace InfoHubPhone8.Config.Implementations
{
    public class ListPageImplementation : PageImplementation
    {
        public ListPaneImplementation Pane { get; set; }


        public ListPageImplementation(IRepository repository, PageDefinition definition)
            : base(repository, definition)
        {
            var def = definition as SinglePanePageDefinition;
            if (def == null) return;
            var paneDef = def.Pane as ListPaneDefinition;
            if (paneDef == null) return;

            Pane = new ListPaneImplementation(repository, paneDef);
        }

    }
}