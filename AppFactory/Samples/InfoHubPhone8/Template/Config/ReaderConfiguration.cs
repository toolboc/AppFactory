using System.Collections.Generic;
using System.Linq;
using InfoHubPhone8.Config.Ads;
using InfoHubPhone8.Config.Content;
using InfoHubPhone8.Config.Content.Lists;
using InfoHubPhone8.Config.Content.Lists.Filters;
using BuiltToRoam;
using InfoHubPhone8.Config.Structure;

namespace InfoHubPhone8.Config
{
    public class ReaderConfiguration
    {
        public string FeedbackEmailAddress { get; set; }

        public bool FullFunctionalityInTrialMode { get; set; }

        public int FullFunctionalityTrialModeLengthInDays { get; set; }

        public int TrialModeLengthInDays { get; set; }

        public string LinkShortener { get; set; }

        public List<SocialType> SocialTypes { get; set; }

        public List<SocialSource> Sources { get; set; }

        public List<SocialProvider> SocialProviders { get; set; }

        public List<IAdProvider> AdProviders { get; set; }

        public IEnumerable<SocialSource> FindDependentSources(IReaderList list)
        {
            var dependent = (from filter in list.Filters
                             let nameContains = filter as NameContainsString
                             let srcName = filter as SourceName
                             let srcType = filter as SourceType
                             from src in Sources
                             where
                                 (nameContains != null && src.SourceName.Contains(nameContains.SearchString)) ||
                                 (srcName != null && srcName.Names.Contains(src.SourceName)) ||
                                 (srcType != null && srcType.Types.Contains(src.SourceTypeName))
                             select src);
            return dependent;
        }

        public IListCreator Lists { get; set; }



        public string TestMode { get; set; }


        public TestModeOptions Testing
        {
            get
            {
#if DEBUG
                return TestMode.EnumParse<TestModeOptions>();
#else
                return TestModeOptions.Disabled;
#endif
            }
            set { TestMode = value.ToString(); }
        }

        public ReaderConfiguration()
        {
            SocialTypes = new List<SocialType>();
            Sources = new List<SocialSource>();
            SocialProviders = new List<SocialProvider>();
            AdProviders = new List<IAdProvider>();
        }

        public PageLayout Structure { get; set; }
    }
}
