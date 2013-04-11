using System;
using System.Linq;
using System.Windows;
using BuiltToRoam.Interfaces;
using BuiltToRoam;
using InfoHubPhone8.Config.Implementations;
using InfoHubPhone8.Config.Structure;
using InfoHubPhone8.Data;
using InfoHubPhone8.Pages.Main;
using InfoHubPhone8.Pages.Reader;

namespace InfoHubPhone8.Pages.List
{
    public class ListPageViewModel : SocialReaderViewModelBase<DefaultPageStates, DefaultAppBarButtons>
    {
        private ListPageImplementation implementation;

        public ListPageImplementation Implementation
        {
            get { return implementation; }
            set
            {
                if (Implementation == value) return;
                implementation = value;
                RaisePropertyChanged(() => Implementation);
            }
        }


        public ListPageViewModel(IRepository repository, IDispatcher dispatcher, INavigation navigation)
            : base(repository, dispatcher, navigation)
        {
            if (IsInDesignMode)
            {
                UpdateDefinitionListName("what's new");
                Implementation = new ListPageImplementation(Repository, Definition);
            }

        }

        private void UpdateDefinitionListName(string newListName)
        {
            var singlePanePageDefinition = Definition as SinglePanePageDefinition;
            if (singlePanePageDefinition == null) return;

            var listPaneDefinition = singlePanePageDefinition.Pane as ListPaneDefinition;
            if (listPaneDefinition == null) return;
            listPaneDefinition.ListName = newListName;

        }

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs navigationEventArgs, System.Windows.Navigation.NavigationContext context, System.Collections.Generic.IDictionary<string, object> State, bool firstLoad)
        {
            base.OnNavigatedTo(navigationEventArgs, context, State, firstLoad);

            var listName = context.QueryString.SafeDictionaryValue<string, string, string>("listname");

            UpdateDefinitionListName(listName);
            Implementation = new ListPageImplementation(Repository, Definition);

            var list = Implementation.Pane.List;
            if (list == null)
            {
                // No list found, so navigate back to the main page
                Navigation.Navigate(typeof(MainPage));
                return;
            }

            RefreshList();
        }

        public void RefreshList()
        {
            Repository.Refresh(Repository.Configuration.FindDependentSources(Implementation.Pane.List.Definition).Select(lst => lst.SourceName).ToArray());
        }

        public void DisplayActivity(SocialActivityWrapper item)
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
                if (Repository.FilteredLists[i].Activities == Implementation.Pane.List.Activities)
                {
                    listIndex = i;
                    break;
                }
            }

            Navigation.Navigate(typeof(ReaderPage),
                new[]
                    {
                        NavigationParameter<string,string>.StringParameter(ReaderPageViewModel.ActivitySourceKey,item.SocialActivity.FeedSourceName),
                        NavigationParameter<string,string>.StringParameter(ReaderPageViewModel.ActivityIdKey,item.SocialActivity.Id),
                        NavigationParameter<string,string>.StringParameter(ReaderPageViewModel.ActvitiyListKey,listIndex.ToString())
                    });
        }

        public void DisplayMainPage()
        {
            Navigation.Navigate(typeof(MainPage));
        }
    }
}
