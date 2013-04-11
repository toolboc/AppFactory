using System.Windows.Controls;
using BuiltToRoam.WindowsPhone.Controls;
using InfoHubPhone8.Config;
using InfoHubPhone8.Config.Content;

namespace InfoHubPhone8.Pages.Settings
{
    public partial class SettingsPage
    {
        // Constructor
        public SettingsPage()
        {
            InitializeComponent();

            var pageAd = ViewModel<SettingsPageViewModel>().PageAd;
            PageHelper.BuildAdControl(pageAd, AdPlaceholder);
        }

        private void NetworkSelected(object sender, SelectionChangedEventArgs e)
        {
            var network = sender.SelectedItem<SocialProvider>();
            if (network == null) return;
            this.ViewModel<SettingsPageViewModel>().AuthenticateNetwork(network);
        }
    }
}