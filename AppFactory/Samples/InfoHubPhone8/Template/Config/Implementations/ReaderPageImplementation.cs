using InfoHubPhone8.Config.Structure;
using InfoHubPhone8.Data;

namespace InfoHubPhone8.Config.Implementations
{
    public class ReaderPageImplementation : PageImplementation
    {
        public ReaderPageImplementation(IRepository repository, PageDefinition definition, ISocialActivityWrapper activity)
            : base(repository, definition)
        {
            Activity = activity;
        }

        public ISocialActivityWrapper Activity { get; private set; }
    }
}