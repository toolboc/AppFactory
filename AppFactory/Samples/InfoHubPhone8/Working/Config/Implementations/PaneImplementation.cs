using InfoHubPhone8.Config.Structure;

namespace InfoHubPhone8.Config.Implementations
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