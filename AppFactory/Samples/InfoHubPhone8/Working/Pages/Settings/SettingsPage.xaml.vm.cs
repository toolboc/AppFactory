using System.Collections.Generic;
using System.Windows;
using System.Linq;
using BuiltToRoam;
using BuiltToRoam.Interfaces;
using InfoHubPhone8.Config;
using InfoHubPhone8.Config.Content;

namespace InfoHubPhone8.Pages.Settings
{

    /// <summary>
    /// View model class for the settings page
    /// </summary>
    public class SettingsPageViewModel : SocialReaderViewModelBase<DefaultPageStates, DefaultAppBarButtons>
    {



        /// <summary>
        /// Initializes a new instance of the SettingsPageViewModel class
        /// </summary>
        /// <param name="repository"></param>
        public SettingsPageViewModel(IRepository repository, IDispatcher dispatcher, INavigation navigation)
            : base(repository, dispatcher, navigation)
        {
        }

        public List<SocialProvider> SocialNetworks
        {
            get
            {
                return (from s in Repository.Configuration.SocialProviders
                        let instance = s.Instance
                        where instance.RequiresAuthentication
                        select s).ToList();
            }
        }



        public void AuthenticateNetwork(SocialProvider network)
        {
            if (network.Instance.IsAuthenticated)
            {
                network.Instance.Disconnect();
                network.UpdateInstance();
            }
            else
            {
                Navigation.Navigate(typeof(Auth.AuthPage), new[] { NavigationParameter<string, string>.StringParameter("network", network.ProviderName) });
            }
        }
    }
}
