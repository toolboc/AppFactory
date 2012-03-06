using Template.Config.Structure;

namespace Template.Config.Implementations
{
    public class LinkPaneImplementation : PaneImplementation
    {

        public LinkPaneImplementation(IRepository repository, LinkPaneDefinition paneDefinition)
            : base(repository, paneDefinition)
        {
        }
    }
}