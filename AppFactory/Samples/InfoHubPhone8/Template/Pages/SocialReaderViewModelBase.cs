using System;
using System.Device.Location;
using System.Reflection;
using System.Windows;
using BuiltToRoam;
using BuiltToRoam.BaseClasses;
using BuiltToRoam.Interfaces;
using InfoHubPhone8.Config;
using InfoHubPhone8.Config.Structure;

namespace InfoHubPhone8.Pages
{
    public class SocialReaderViewModelBase<TPageStates, TAppBarButtons> : ApplicationBarStateViewModelBase<IRepository, TPageStates, TAppBarButtons>
        where TPageStates : struct
        where TAppBarButtons : struct
    {

        protected ReaderConfiguration Configuration { get; private set; }

        private PageDefinition definition;
        protected PageDefinition Definition
        {
            get
            {
                if (definition == null)
                {
                    definition =
                        Repository.Configuration.Structure.FindDefinitionByType(GetType());
                }
                return definition;
            }
        }

        private GeoCoordinate currentLocation = new GeoCoordinate();
        public GeoCoordinate CurrentLocation
        {
            get { return currentLocation; }
        }

        private PageAdInformation pageAd;
        public virtual PageAdInformation PageAd
        {
            get
            {
                if (pageAd == null)
                {
                    pageAd = Repository.PageAd(GetType());
                }

                // If null is returned then return a new instance, which will
                // set Ad visibility to collapsed by default
                if (pageAd == null) return new PageAdInformation();

                return pageAd;
            }
        }

        public override bool IsInTrialMode
        {
            get
            {
                return Repository.IsInTrialMode;
            }
        }

        public virtual string ApplicationTitle
        {
            get
            {
                return Utilities.ApplicationTitle.ToUpper();
            }
        }

        public SocialReaderViewModelBase(IRepository repository, IDispatcher dispatcher, INavigation navigation)
            : base(repository, dispatcher, navigation)
        {
            // Wire event handlers for the repository status events
            AddEventWiring(() => Repository.IsLoadingChanged += RepositoryIsLoadingChanged, () => Repository.IsLoadingChanged -= RepositoryIsLoadingChanged);
            AddEventWiring(() => Repository.IsUpdatingChanged += RepositoryIsLoadingChanged, () => Repository.IsUpdatingChanged -= RepositoryIsLoadingChanged);

            IsDataLoading = Repository.IsUpdating || Repository.IsLoading;
            Configuration = Application.Current.Resources["Configuration"] as ReaderConfiguration;
        }

        /// <summary>
        /// Handles changes in the loading status of the repository.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RepositoryIsLoadingChanged(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() => IsDataLoading = Repository.IsUpdating || Repository.IsLoading);
        }

        public void LaunchLink(LinkDefinition link)
        {
            switch (link.Type)
            {
                case LinkType.Page:
                    var typeName = link.Destination;
                    var type = Assembly.GetExecutingAssembly().GetType(typeName);
                    Navigation.Navigate(type,
                                        new INavigationParameter[]
                                            {
                                                NavigationParameter<string, string>.StringParameter("link", link.Parameter)
                                            });
                    break;
                case LinkType.Site:
                    Utilities.DisplayInWebBrowser(link.Destination);
                    break;
                case LinkType.Custom:
                    OnCustomLaunchLink(link);
                    break;
            }
        }

        protected virtual void OnCustomLaunchLink(LinkDefinition link)
        {

        }
    }
}
