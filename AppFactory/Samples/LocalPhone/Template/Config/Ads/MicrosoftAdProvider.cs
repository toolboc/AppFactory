using System.Collections.Generic;
using System.Windows.Markup;

namespace Template.Config.Ads
{
    [ContentProperty("AdUnits")]
    public class MicrosoftAdProvider : IAdProvider
    {
        public string AdProviderName { get; set; }
        public string AdApplicationId { get; set; }

        public List<IAdUnit> AdUnits { get; set; }
        public MicrosoftAdProvider()
        {
            AdUnits = new List<IAdUnit>();
        }
    }
}