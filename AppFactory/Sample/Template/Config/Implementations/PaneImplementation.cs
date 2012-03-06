using Template.Config.Structure;

namespace Template.Config.Implementations
{
    public abstract class PaneImplementation
    {
        public IRepository Repository { get; set; }
        public PaneDefinition Definition { get; set; }

        protected PaneImplementation(IRepository repository, PaneDefinition paneDefinition)
        {
            Repository = repository;
            Definition = paneDefinition;
        }
    }
}