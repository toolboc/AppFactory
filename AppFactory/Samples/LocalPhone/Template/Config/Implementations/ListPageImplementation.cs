using Template.Config.Content.Lists;
using Template.Config.Structure;

namespace Template.Config.Implementations
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