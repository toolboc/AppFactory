using System.Collections.Generic;

namespace Template.Config.Ads
{
    public interface IAdProvider
    {
        string AdProviderName { get; set; }
        List<IAdUnit> AdUnits { get; set; }
    }
}