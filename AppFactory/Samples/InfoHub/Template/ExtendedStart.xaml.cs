using System;
using System.Diagnostics;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;

namespace InfoHub
{
    public sealed partial class ExtendedStart : Page
    {
        public ExtendedStart()
        {
            this.InitializeComponent();
            Loaded += async (s, e) =>
            {
                if (InfoHub.AppHubViewModel.Strings.IncludeAdvertising)
                    try
                    {
                        var _Geo = new Geolocator();
                        await _Geo.GetGeopositionAsync();
                    }
                    catch
                    {
                        // this breakpoint may be reached if the user BLOCKS location services
                        Debug.WriteLine("Error reading location; user may have BLOCKED");
                        System.Diagnostics.Debugger.Break();
                    }
                this.Frame.Navigate(typeof(AppHub));
            };
        }
    }
}
