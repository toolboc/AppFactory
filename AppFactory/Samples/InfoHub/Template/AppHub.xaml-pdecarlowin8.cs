using System.Diagnostics;
using InfoHub.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace InfoHub
{
    public sealed partial class AppHub : InfoHub.Common.LayoutAwarePage
    {
        public AppHub()
        {
            this.InitializeComponent();
            HubGridView.Margin = new Thickness(0);
            Loaded += AppHub_Loaded;
            MyBrowser.BackClick += MyBrowser_BackClick;
        }

        void MyBrowser_BackClick(object sender, EventArgs e)
        {
            ActivateAds();
        }

        AppHubViewModel ViewModel { get { return this.DataContext as AppHubViewModel; } }

        async void AppHub_Loaded(object sender, RoutedEventArgs e)
        {
            GridViewOut.ItemsSource = cvs.View.CollectionGroups;
            await ViewModel.Start();

            // are we enabling advertising?
            if (!AppHubViewModel.Strings.IncludeAdvertising)
                this.MyPurchaseButton.Visibility = Visibility.Collapsed;
            else
            {
                ViewModel.PropertyChanged += (s, args) =>
                {
                    if (!new string[] { "Longitude", "Latitude" }.Contains(args.PropertyName))
                        return;
                    UpdateAdvertisements();
                };
                UpdateAdvertisements();
            }

        }

        private void UpdateAdvertisements()
        {
            Debug.Assert(HubGridView.Items != null, "HubGridView.Items != null");
            foreach (var _Item in HubGridView.Items)
            {
                var _Container = HubGridView.ItemContainerGenerator.ContainerFromItem(_Item);
                var _Children = AllChildren(_Container);
                foreach (var _AdControl in _Children
                    .OfType<Microsoft.Advertising.WinRT.UI.AdControl>())
                {
                    _AdControl.Latitude = ViewModel.Latitude;
                    _AdControl.Longitude = ViewModel.Longitude;
                    _AdControl.ApplicationId = AppHubViewModel.Strings.AdApplicationId;
                    _AdControl.AdUnitId = AppHubViewModel.Strings.AdUnitId;
                }
            }
        }

        public List<Control> AllChildren(DependencyObject parent)
        {
            if (parent == null)
                return (new Control[] { }).ToList();
            var _List = new List<Control> { };
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var _Child = VisualTreeHelper.GetChild(parent, i);
                if (_Child is Control)
                    _List.Add(_Child as Control);
                _List.AddRange(AllChildren(_Child));
            }
            return _List;
        }

        async void HubGridView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            // can't get more ifthere's no internet
            if (await IsInternet())
            {
                var _Item = e.ClickedItem as IArticle;
                if (_Item is AdvertArticle)
                    return;
                else if (_Item is SummaryArticle)
                    return;
                else if (_Item is CalendarArticle)
                    return;
                else if (_Item is WeatherArticle)
                    return;
                else
                {
                    var _AppHubViewModel = this.DataContext as AppHubViewModel;
                    if (_AppHubViewModel != null)
                    {
                        var _Feed = _AppHubViewModel
                            .Feeds.First(x => x.Articles.Contains(_Item)).Feed;
                        MyBrowser.Navigate(_Item, _Feed);
                        DeactivateAds();
                    }
                }
            }
        }

        public void ActivateAds()
        {
            Debug.Assert(HubGridView.Items != null, "HubGridView.Items != null");
            foreach (var _Item in HubGridView.Items)
            {
                var _Container = HubGridView.ItemContainerGenerator.ContainerFromItem(_Item);
                var _Children = AllChildren(_Container);
                foreach (var _AdControl in _Children.OfType<Microsoft.Advertising.WinRT.UI.AdControl>())
                {
                    _AdControl.Visibility = Visibility.Visible;
                }
            }
        }

        public void DeactivateAds()
        {
            Debug.Assert(HubGridView.Items != null, "HubGridView.Items != null");
            foreach (var _Item in HubGridView.Items)
            {
                var _Container = HubGridView.ItemContainerGenerator.ContainerFromItem(_Item);
                var _Children = AllChildren(_Container);
                foreach (var _AdControl in _Children.OfType<Microsoft.Advertising.WinRT.UI.AdControl>())
                {
                    _AdControl.Visibility = Visibility.Collapsed;
                }
            }
        }

        public static async System.Threading.Tasks.Task<bool> IsInternet()
        {
            var _InternetConnectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            var _IsInernet = _InternetConnectionProfile != null;
            if (!_IsInernet)
                await new Windows.UI.Popups.MessageDialog(
                    content: InfoHub.AppHubViewModel.Strings.NoInternetWarning,
                    title: InfoHub.AppHubViewModel.Strings.NoInternetWarning).ShowAsync();
            return _IsInernet;
        }

        async private void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            try
            {
                var _Article = e.ClickedItem as IArticle;
                if (_Article != null)
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(_Article.Url));
            }
            catch { }
        }
    }
}
