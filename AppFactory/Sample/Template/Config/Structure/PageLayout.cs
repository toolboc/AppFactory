using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Template.Config.Structure
{
    [ContentProperty("Pages")]
    public class PageLayout
    {
        public List<PageDefinition> Pages { get; set; }

        public PageLayout()
        {
            Pages = new List<PageDefinition>();
        }

        public PageDefinition FindDefinitionByType(Type lookupType)
        {
            return Pages.First(p => (p.PageName == lookupType.Name));
        }
    }
}