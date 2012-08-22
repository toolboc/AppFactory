using Template.Config.Structure;
using Template.Data;

namespace Template.Config.Implementations
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