using InfoHubPhone8.Config.Structure;

namespace InfoHubPhone8.Config.Implementations
{
    public class PageImplementation
    {
        public PageDefinition Definition { get; private set; }

        public IRepository Repository { get; private set; }

        public PageImplementation(IRepository repository, PageDefinition definition)
        {
            Definition = definition;
            Repository = repository;
        }
    }
}