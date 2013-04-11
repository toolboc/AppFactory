using BuiltToRoam;

namespace InfoHubPhone8.Pages.About
{
    public partial class AboutPage
    {
        // Constructor
        public AboutPage()
        {
            InitializeComponent();

            var pageAd = ViewModel<AboutPageViewModel>().PageAd;
            PageHelper.BuildAdControl(pageAd, AdPlaceholder);
        }

        private void PurchaseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Utilities.LaunchMarketplaceDetails();
        }

        private void ReviewClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Utilities.LaunchMarketplaceReview();
        }

        private void FeedbackClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.DataContext as AboutPageViewModel).LaunchFeedback();

        }
    }
}