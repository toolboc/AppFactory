using System.Linq;
using BuiltToRoam;
using BuiltToRoam.Interfaces;
using InfoHubPhone8.Config;
using InfoHubPhone8.Config.Content;

namespace InfoHubPhone8.Pages.Auth
{
    public class AuthPageViewModel : SocialReaderViewModelBase<DefaultPageStates, DefaultAppBarButtons>
    {
        public SocialProvider Network { get; set; }

        public AuthPageViewModel(IRepository repository, IDispatcher dispatcher, INavigation navigation)
            : base(repository, dispatcher, navigation)
        {
        }

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs navigationEventArgs, System.Windows.Navigation.NavigationContext context, System.Collections.Generic.IDictionary<string, object> State, bool firstLoad)
        {
            base.OnNavigatedTo(navigationEventArgs, context, State, firstLoad);

            var networkName = context.QueryString["network"];
            Network = (from s in Repository.Configuration.SocialProviders
                       where s.ProviderName == networkName
                       select s).FirstOrDefault();

        }
    }
}
