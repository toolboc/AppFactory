namespace Template.Pages.Auth
{
    public partial class AuthPage
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel<AuthPageViewModel>().Network.Instance.AuthenticationCompleted += SocialNetwork_AuthenticationCompleted;
            ViewModel<AuthPageViewModel>().Network.Instance.Authenticate(Browser);
        }

        void SocialNetwork_AuthenticationCompleted(object sender, BuiltToRoam.DualParameterEventArgs<bool, System.Exception> e)
        {
            this.Dispatcher.BeginInvoke(() =>
                                            {
                                                // Force updates to any UI bound to the social network instance
                                                // Done here to make sure it's done on the UI thread
                                                ViewModel<AuthPageViewModel>().Network.UpdateInstance();

                                                this.NavigationService.GoBack();
                                            });
        }
    }
}