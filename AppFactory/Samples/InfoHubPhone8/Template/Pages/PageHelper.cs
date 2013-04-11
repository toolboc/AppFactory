using System.Windows;
using System.Windows.Controls;
using Microsoft.Advertising.Mobile.UI;
using InfoHubPhone8.Config.Ads;


namespace InfoHubPhone8.Pages
{
    public static class PageHelper
    {
        public static void BuildAdControl(PageAdInformation pageAd, Panel adControlHost)
        {
            if (pageAd.AdsVisibility == Visibility.Visible)
            {
                var adControl = new AdControl()
                {
#if DEBUG
                    ApplicationId="test_client",
                    AdUnitId = "Image480_80",
#else
                    ApplicationId = (pageAd.Provider as MicrosoftAdProvider).AdApplicationId,
                    AdUnitId = (pageAd.Unit as MicrosoftAdUnit).AdUnitId,
#endif
                    Height = 80,
                    Width = 480,
                    Visibility = Visibility.Visible
                };
                adControlHost.Children.Add(adControl);
            }
        }

    }
}
