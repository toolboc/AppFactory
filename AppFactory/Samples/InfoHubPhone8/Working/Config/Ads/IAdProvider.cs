using System.Collections.Generic;

namespace InfoHubPhone8.Config.Ads
{
    public interface IAdProvider
    {
        string AdProviderName { get; set; }
        List<IAdUnit> AdUnits { get; set; }
    }
}