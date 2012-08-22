using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using BuiltToRoam;
using BuiltToRoam.Interfaces;
using Microsoft.Phone.Shell;
using Template.Config.Implementations;
using Template.Config.Structure;
using Template.Data;
using Template.Pages.Reader;
using Template.Pages.Settings;

namespace Template.Pages.Main
{
    public enum MainPageStates
    {
        Base,
        AppBarDefault,
        AppBarMinimized,
        AppBarNone
    }

    public enum MainPageAppBarItems
    {
        refresh,
        pin,
        unpin
    }

    public class MainPageViewModel : SocialReaderViewModelBase<MainPageStates, MainPageAppBarItems>
    {

        public MultiplePanePageImplementation PanoramaImplementation { get; private set; }

        public MainPageViewModel(IRepository repository, IDispatcher dispatcher, INavigation navigation)
            : base(repository, dispatcher, navigation)
        {
            StateDefinitions = new Dictionary
                <MainPageStates, ApplicationBarStateDefinition<MainPageStates, MainPageAppBarItems>>()
                                   {
                                       {MainPageStates.AppBarDefault, new ApplicationBarStateDefinition<MainPageStates, MainPageAppBarItems>(
                                           ()=>Visibility.Visible, 
                                           ()=>ApplicationBarMode.Default,new Dictionary<MainPageAppBarItems, Func<bool>>
                                                                              {
                                                                                  {MainPageAppBarItems.refresh, ()=>true},
                                                                                  {MainPageAppBarItems.pin, ()=>!CurrentListIsPinned},
                                                                                  {MainPageAppBarItems.unpin, ()=>CurrentListIsPinned}
                                                                              }
                                           )},
                                           {MainPageStates.AppBarMinimized, new ApplicationBarStateDefinition<MainPageStates, MainPageAppBarItems>(
                                           ()=>Visibility.Visible, 
                                           ()=>ApplicationBarMode.Minimized,new Dictionary<MainPageAppBarItems, Func<bool>>
                                                                              {
                                                                                  {MainPageAppBarItems.refresh, ()=>true},
                                                                                  {MainPageAppBarItems.pin, ()=>!CurrentListIsPinned},
                                                                                  {MainPageAppBarItems.unpin, ()=>CurrentListIsPinned}
                                                                              }
                                           )},
                                            {MainPageStates.AppBarNone, new ApplicationBarStateDefinition<MainPageStates, MainPageAppBarItems>(
                                           ()=>Visibility.Collapsed, 
                                           null,null // Don't need to configure individual buttons since the app bar is collapsed
                                           )}
                                   };


            // (Optional) This sets the minimum latency between SocialFeedLoaded events. Increasing
            // this will increase load time for all feed data but will allow the UI
            // to be more responsive
            Repository.UpdateThresholdInMilliseconds = 100;

            // (Optional) This is the maximum number of social activities that will be cached.
            Repository.MaximumActivitiesToCache = 50;

            // Wire up event for when items are retrieved from feeds
            Repository.SocialFeedLoaded += SocialFeedLoaded;

            var def = Definition as MultiplePanePageDefinition;

            PanoramaImplementation = new MultiplePanePageImplementation(repository, def);
        }

        public override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs navigatingCancelEventArgs, IDictionary<string, object> state)
        {
            base.OnNavigatingFrom(navigatingCancelEventArgs, state);

            if (!navigatingCancelEventArgs.Cancel && navigatingCancelEventArgs.NavigationMode == NavigationMode.New)
            {
                // When navigating away from the main page, make sure
                // it is the first page on the back stack. For example
                // the user may have entered the app via the List page
                while (Navigation.BackStack.FirstOrDefault() != null)
                {
                    Navigation.RemoveBackEntry();
                }
            }
        }

        public override void OnNavigatedTo(NavigationEventArgs navigationEventArgs, NavigationContext context, IDictionary<string, object> State, bool firstLoad)
        {
            base.OnNavigatedTo(navigationEventArgs, context, State, firstLoad);

            RefreshApplicationBarState();
        }


        protected bool CurrentListIsPinned
        {
            get
            {
                if (SelectedPane == null) return false;
                var listDef = SelectedPane.Definition as ListPaneDefinition;
                if (listDef == null) return false;
                var listName = listDef.ListName;
                return Repository.IsPinned(listName);
            }
        }


        /// <summary>
        /// Add content to this method to dynamically do stuff with new activities
        /// </summary>
        private void SocialFeedLoaded(object sender, ParameterEventArgs<ISocialActivityWrapper> e)
        {

        }

        /// <summary>
        /// Call this method to refresh activities
        /// </summary>
        public void RefreshActivities()
        {
            // You can specify "on demand" sources as an array of strings 
            // as a parameter to the Refresh method eg:
            // Repository.Refresh("Event","Twitter Data");
            // The strings are Source Names from the configuration data (in App.xaml)
            Repository.Refresh();
        }

        /// <summary>
        /// Call this method to get more items for the activity sources
        /// </summary>
        public void MoreActivities()
        {
            Repository.LoadMore();
        }


        /// <summary>
        /// Triggers the social activity to be displayed in the reader page
        /// </summary>
        /// <param name="item"></param>
        /// <param name="list"></param>
        public void DisplaySocialActivity(ISocialActivityWrapper item, IEnumerable<ISocialActivityWrapper> list)
        {
            // If the app is in trial mode, warn the user
            if (!Repository.IsInFullFunctionalityMode)
            {
                if (Repository.IsInTrialPeriod)
                {
                    var days = Repository.TrialExpiresOn.Date.Subtract(DateTime.Now.Date).Days;
                    var daysToGo = days + (days > 1 ? " days" : " day");

                    MessageBox.Show("You have " + daysToGo + " remaining until the trial period expires");
                }
                else
                {
                    MessageBox.Show(
                        "Trial mode has expired, please purchage this application in order to view item details");
                    return;
                }
            }


            // Find the index of the selected item
            var listIndex = -1;
            for (int i = 0; i < Repository.FilteredLists.Count; i++)
            {
                if (Repository.FilteredLists[i].Activities == list)
                {
                    listIndex = i;
                    break;
                }
            }

            //// Add activity source and the item id to the query string (to make sure the activity item can be located)
            //var query = string.Format(ReaderPageViewModel.ReaderPageUrlTemplate, item.SocialActivity.FeedSource.Source, HttpUtility.UrlEncode(item.SocialActivity.Id), listIndex);


            //// Navigate to the Reader page - not that this includes the folder location of the page
            //this.Navigation.Navigate(new Uri(query, UriKind.RelativeOrAbsolute));

            Navigation.Navigate(typeof(ReaderPage),
    new[]
                    {
                        NavigationParameter<string,string>.StringParameter(ReaderPageViewModel.ActivitySourceKey,item.SocialActivity.FeedSourceName),
                        NavigationParameter<string,string>.StringParameter(ReaderPageViewModel.ActivityIdKey,item.SocialActivity.Id),
                        NavigationParameter<string,string>.StringParameter(ReaderPageViewModel.ActvitiyListKey,listIndex.ToString())
                    });
        }

        /// <summary>
        /// Handles when the view model has loaded completely - an opportunity to
        /// update UI 
        /// </summary>
        /// <param name="isFirstLoad"></param>
        public override void ViewModelLoadComplete(bool isFirstLoad)
        {
            base.ViewModelLoadComplete(isFirstLoad);

            if (!IsInTrialMode)
            {
                var custom = Definition as MultiplePanePageDefinition;
                var links = (from pane in custom.Panes.OfType<LinkPaneDefinition>()
                             select pane).FirstOrDefault();
                var purchaseLink = (from link in links.Links
                                    where link.Title == "purchase"
                                    select link).FirstOrDefault();
                purchaseLink.Visibility = Visibility.Collapsed;
            }

            if (CurrentApplicationBarState == null)
            {
                ChangePanoramaSelection(PanoramaImplementation.Items[0]);
            }

            //if (isFirstLoad)
            //{
            //    TiledActivities=(from list in Repository.FilteredLists
            //                     where list.ListName == Repository.Configuration.Layout.PanoramaSecondListName
            //            select list).FirstOrDefault();
            //    ListActivities = (from list in Repository.FilteredLists
            //                      where list.ListName == Repository.Configuration.Layout.PanoramaFirstListName
            //                       select list).FirstOrDefault();
            //}

        }

        //public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs navigationEventArgs, System.Windows.Navigation.NavigationContext context, IDictionary<string, object> State, bool firstLoad)
        //{
        //    base.OnNavigatedTo(navigationEventArgs, context, State, firstLoad);

        //    ChangeApplicationBarState(MainPageStates.AppBarMinimized);

        //}


        /// <summary>
        /// Displays the settings page
        /// </summary>
        public void DisplaySettings()
        {
            Navigation.Navigate(typeof(SettingsPage));
        }

        protected override void OnCustomLaunchLink(LinkDefinition link)
        {
            base.OnCustomLaunchLink(link);

            if (link.Title.ToLower() == "purchase")
            {
                Utilities.LaunchMarketplaceDetails();
                return;
            }

        }

        private PaneImplementation SelectedPane { get; set; }

        public void ChangePanoramaSelection(PaneImplementation paneImplementation)
        {
            SelectedPane = paneImplementation;
            if (SelectedPane == null) return;
            var newAppBarState = SelectedPane.Definition.AppBarStateName.EnumParse<MainPageStates>();
            ChangeApplicationBarState(newAppBarState);
        }

        public void PinList(PaneImplementation paneDefinition)
        {
            var listDef = paneDefinition.Definition as ListPaneDefinition;
            if (listDef == null) return;
            Repository.PinActivityListToStart(listDef.ListName);
            // When the user pins the list they will be directed to the Start
            // screen, so we'll update the application bar in the OnNavigateTo
            // method when the user returns
        }

        public void UnPinList(PaneImplementation paneDefinition)
        {
            var listDef = paneDefinition.Definition as ListPaneDefinition;
            if (listDef == null) return;
            Repository.UnPinActivityListToStart(listDef.ListName);
            // Need to refresh the application bar to show the pin icon again
            RefreshApplicationBarState();
        }
    }
}
