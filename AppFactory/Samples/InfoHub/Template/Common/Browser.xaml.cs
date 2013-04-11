using System.Diagnostics;
using InfoHub.Articles;
using InfoHub.Feeds;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace InfoHub.Common
{
    public sealed partial class Browser : UserControl
    {
        public event EventHandler BackClick;

        public Browser()
        {
            this.InitializeComponent();
            Loaded += (s, e) => this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

        }

        protected void OnBackClick(EventArgs e)
        {
            EventHandler handler = BackClick;
            if (handler != null)
            {
                // Invokes the delegates.
                handler(this, e);
            }
        }

        private void MyBackButton_Click_1(object sender, RoutedEventArgs e)
        {
            Navigate("about:blank");
            this.Visibility = Visibility.Collapsed;
            OnBackClick(EventArgs.Empty);
        }

        public void Navigate(string uri)
        {
            try
            {
                MyWebView.Navigate(new Uri(uri));
                this.Visibility = Visibility.Visible;
            }
            catch
            {
                new Windows.UI.Popups.MessageDialog("Sorry. That feed's Url is invalid. :(").ShowAsync();
                this.Visibility = Visibility.Collapsed;
            }
        }

        public void Navigate(IArticle item, IFeed feed)
        {
            // MyWebView.NavigateToString(item.Body);
            Navigate(item.Url);

            ItemsList.ItemsSource = feed.Articles.OrderByDescending(x => x.Date);
            ItemsList.SelectedItem = item;
            ItemsList.ScrollIntoView(item);
        }

        private void ItemsList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.Assert(e.OriginalSource != null, "e.OriginalSource != null");
            var _FrameworkElement = e.OriginalSource as FrameworkElement;
            if (_FrameworkElement != null)
            {
                var _Item = _FrameworkElement.DataContext as IArticle;
                Debug.Assert(_Item != null, "_Item != null");
                
                if (_Item == null)
                    return;
                
                MyWebView.Navigate(new Uri(_Item.Url));
            }
            MyProgressRing.Visibility = Visibility.Visible;
        }

        private void MyWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            MyProgressRing.Visibility = Visibility.Collapsed;
        }
    }
}
