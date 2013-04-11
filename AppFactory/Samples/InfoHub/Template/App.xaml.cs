using System;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InfoHub
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Debug.Assert(args != null, "args != null");
            Init(args.Arguments);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            Init(null);
            Debug.Assert(args != null, "args != null");
            base.OnActivated(args);
        }

        private static void Init(object args)
        {
            var  _RootFrame = Window.Current.Content as Frame;
            if (_RootFrame == null)
            {
                _RootFrame = new Frame();
                Window.Current.Content = _RootFrame;
            }
            if (_RootFrame.Content == null)
            {
                if (!_RootFrame.Navigate(typeof(ExtendedStart), args))
                    throw new Exception("Failed to create initial page");
            }
            Window.Current.Activate();
        }

        public static bool IsNetworkOkay
        {
            // Windows.Networking.Connectivity.NetworkInformation.NetworkStatusChanged
            get
            {
                var _Profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                if (_Profile == null)
                    return false;
                return _Profile.GetNetworkConnectivityLevel() == Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess;
            }
        }
    }
}
