using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using BuiltToRoam;
using BuiltToRoam.Interfaces;
using InfoHubPhone8.Config;
using InfoHubPhone8.Config.Content.Lists;
using InfoHubPhone8.Config.Implementations;
using InfoHubPhone8.Config.Structure;
using InfoHubPhone8.Pages;
using InfoHubPhone8.Data;
using InfoHubPhone8.Pages.Main;
using InfoHubPhone8.Pages.Reader;

namespace InfoHubPhone8.Pages.Pivot
{
    public class PivotPageViewModel : SocialReaderViewModelBase<DefaultPageStates, DefaultAppBarButtons>
    {
        private const string ListsRefreshedKey = "ListRefreshed";
        /// <summary>
        /// This prevent unnecessary downloading of content when the user is swiping rapidly through the pivot items
        /// </summary>
        private const int PivotSelectedTimeoutInMilliseconds = 500;
        ///// <summary>
        ///// This prevents an exception being raised when the pivot page is first loaded and an 
        ///// </summary>
        //public const int PivotSelectionTimeoutInMilliseconds = 200;

        private List<FilteredActivityList> pivotItems;

        public List<FilteredActivityList> PivotItems
        {
            get { return pivotItems; }
            set
            {
                if (PivotItems == value) return;
                pivotItems = value;
                RaisePropertyChanged(() => PivotItems);
            }
        }

        private Dictionary<string, bool> listRefreshed;

        public override PageAdInformation PageAd
        {
            get
            {
                var pageAd = base.PageAd;

                //if (!(Repository.IsInLimitedFunctionalityMode || Repository.IsInTrialExpiredMode))
                //{
                //    pageAd.Provider = null;
                //    pageAd.Unit = null;
                //}

                return pageAd;
            }
        }

        public MultiplePanePageImplementation PivotImplementation { get; private set; }


        public PivotPageViewModel(IRepository repository, IDispatcher dispatcher, INavigation navigation)
            : base(repository, dispatcher, navigation)
        {
            //if (DesignerProperties.IsInDesignTool)
            //{
            //    PivotItems = (from list in Repository.FilteredLists
            //                  where Repository.Configuration.Layout.PivotListNames.Contains(list.ListName)
            //                  select list).ToList();
            //}

            var def = Definition as MultiplePanePageDefinition;

            PivotImplementation = new MultiplePanePageImplementation(repository, def);
        }

        private FilteredActivityList currentPivot;
        public void DownloadPivotContent(FilteredActivityList list)
        {

            // Check if this list has previously been refreshed on this page
            var refreshed = listRefreshed.SafeDictionaryValue<string, bool, bool>(list.ListName);
            if (refreshed)
            {
                return;
            }

            currentPivot = list;

            ThreadPool.QueueUserWorkItem(async =>
                                             {
                                                 var pivot = async as FilteredActivityList;
                                                 Thread.Sleep(PivotSelectedTimeoutInMilliseconds);
                                                 if (currentPivot != pivot) return;
                                                 var name = currentPivot.ListName;
                                                 var lists = Repository.Configuration.FindDependentSources(pivot.Definition).Select(src => src.SourceName).ToArray();
                                                 //var lists = (from src in Repository.Configuration.Sources
                                                 //             where src.Name.ToLower().Contains(name.ToLower())
                                                 //             select src.Name).ToArray();
                                                 Repository.Refresh(lists);
                                                 listRefreshed[currentPivot.ListName] = true;
                                             }, list);


        }

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

            Navigation.Navigate(typeof(ReaderPage),
                new[]
                    {
                        NavigationParameter<string,string>.StringParameter(ReaderPageViewModel.ActivitySourceKey,item.SocialActivity.FeedSourceName),
                        NavigationParameter<string,string>.StringParameter(ReaderPageViewModel.ActivityIdKey,item.SocialActivity.Id),
                        NavigationParameter<string,string>.StringParameter(ReaderPageViewModel.ActvitiyListKey,listIndex.ToString())
                    });
        }

        public void MoreActivities(FilteredActivityList source)
        {
            var sources = Repository.Configuration.FindDependentSources(source.Definition);
            Repository.LoadMore(sources.Select(src => src.SourceName).ToArray());
        }

        public override void OnNavigatedFrom(NavigationEventArgs navigationEventArgs, IDictionary<string, object> State)
        {
            base.OnNavigatedFrom(navigationEventArgs, State);

            State[ListsRefreshedKey] = listRefreshed;
        }

        public override void OnNavigatedTo(NavigationEventArgs navigationEventArgs, NavigationContext context, IDictionary<string, object> State, bool firstLoad)
        {
            base.OnNavigatedTo(navigationEventArgs, context, State, firstLoad);

            listRefreshed = State.SafeDictionaryValue<string, object, Dictionary<string, bool>>(ListsRefreshedKey);
            if (listRefreshed == null)
            {
                listRefreshed = new Dictionary<string, bool>();
            }


            var query = context.QueryString;

            if (PivotItems == null)
            {



                var listName = query["link"] as string;

                var listimp = (from lst in PivotImplementation.Items
                               let listDef = lst.Definition as ListPaneDefinition
                               where listDef != null && listDef.ListName == listName
                               select lst).FirstOrDefault();
                PivotImplementation.Items.Remove(listimp);
                PivotImplementation.Items.Insert(0, listimp);
                DownloadPivotContent((listimp as ListPaneImplementation).List);

                //var lists = (from list in Repository.FilteredLists
                //             where Repository.Configuration.Layout.PivotListNames.Contains(list.ListName)
                //             select list).ToList();

                //var selectedList = (from fal in lists
                //                    where fal.ListName == listName
                //                    select fal).FirstOrDefault();
                //if (selectedList != null)
                //{
                //    lists.Remove(selectedList);
                //    lists.Insert(0, selectedList);
                //}

                //ThreadPool.QueueUserWorkItem(async =>
                //                                 {
                //                                     Thread.Sleep(PivotSelectionTimeoutInMilliseconds);
                //                                     this.BeginInvoke(() =>
                //                                                          {
                //PivotItems = lists;
                //DownloadPivotContent(selectedList);
                //                         });
                //});
            }
        }



        public void RefreshActivities(FilteredActivityList source)
        {
            listRefreshed[source.ListName] = true;

            var sources = (from r in Repository.Configuration.Sources
                           where r.SourceName.Contains(source.ListName)
                           select r.SourceName).ToArray();

            Repository.Refresh(sources);
        }

        //public void Save(IDictionary<string, object> dictionary)
        //{
        //    dictionary[ListsRefreshedKey] = listRefreshed;
        //}

        //public void Restore(IDictionary<string, object> dictionary)
        //{
        //    listRefreshed = dictionary.SafeDictionaryValue<string, object, Dictionary<string, bool>>(ListsRefreshedKey);
        //    if (listRefreshed == null)
        //    {
        //        listRefreshed=new Dictionary<string, bool>();
        //    }
        //}

        //public void CurrentStates(IDictionary<string, string> controlStates)
        //{
        //}
    }
}
