using BuiltToRoam;

namespace InfoHubPhone8.Config.Structure
{
    public class PageDefinition
    {
        public string PageType { get; set; }

        public string PageName { get; set; }

        public string AdProviderName { get; set; }

        public string AdUnitName { get; set; }

        public PageType Type
        {
            get
            {
                return PageType.EnumParse<PageType>();
            }
        }
    }
}