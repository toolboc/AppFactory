using System.Collections.Generic;
using System.Windows.Markup;

namespace InfoHubPhone8.Config.Structure
{
    [ContentProperty("ReaderTemplates")]
    public class ReaderPageDefinition : PageDefinition
    {
        public List<ReaderTemplate> ReaderTemplates { get; set; }

        public ReaderPageDefinition()
        {
            ReaderTemplates = new List<ReaderTemplate>();
        }
    }
}