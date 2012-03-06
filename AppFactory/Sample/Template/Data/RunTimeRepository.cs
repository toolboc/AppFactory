using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using BuiltToRoam;
using BuiltToRoam.Social;
using Microsoft.Phone.Data.Linq;
using Template.Config;
using Template.Config.Content;
using Template.Config.Content.Lists;
using Template.Parsers;
using Template.SourceUrls;
using Microsoft.Phone.Shell;

namespace Template.Data
{
    public class RunTimeRepository : IRepository
    {
        private const string ActivitiesFileName = "socialactivities.sdf";
        public int MaximumActivitiesToCache { get; set; }
        public int UpdateThresholdInMilliseconds { get; set; }

        public bool IsUpdating { get; set; }
        public event EventHandler IsUpdatingChanged;

        public bool IsLoading { get; set; }
        public event EventHandler IsLoadingChanged;

        private Activities Connect()
        {
            return new Activities("Data Source=isostore:/" + ActivitiesFileName);
        }


        protected IDictionary<string, ISourceUrl> SourceUrls { get; private set; }

        protected IDictionary<string, ISourceParser> Parsers { get; private set; }

        private List<FilteredActivityList> filteredLists;
        public List<FilteredActivityList> FilteredLists
        {
            get
            {
                if (filteredLists == null)
                {
                    filteredLists = new List<FilteredActivityList>();
                    RestoreActivitiesFromStorage();
                }
                return filteredLists;
            }
        }


        private ReaderConfiguration configuration;
        private bool listsCreated = false;
        public ReaderConfiguration Configuration
        {
            get { return configuration; }
            set
            {
                configuration = value;

                if (!DesignerProperties.IsInDesignTool)
                {
                    firstRun = ReaderSettings.Instance.FirstRun;

                }

            }
        }



        public void Refresh(params string[] onDemandSourcesByName)
        {
            loadMoreResults = false;
            ThreadPool.QueueUserWorkItem((async) => LoadConfiguration(onDemandSourcesByName));
        }

        public void LoadMore(params string[] onDemandSourcesByName)
        {
            loadMoreResults = true;
            ThreadPool.QueueUserWorkItem((async) => LoadConfiguration(onDemandSourcesByName));
        }

        public event EventHandler<ParameterEventArgs<ISocialActivityWrapper>> SocialFeedLoaded;

        private ObservableCollection<ISocialActivityWrapper> activities;
        public ObservableCollection<ISocialActivityWrapper> Activities
        {
            get
            {
                if (activities == null)
                {
                    RestoreActivitiesFromStorage();
                }
                return activities;
            }
        }

        private int loadLock = 0;
        private AutoResetEvent requestEvent = new AutoResetEvent(false);


        public RunTimeRepository()
        {
            MaximumActivitiesToCache = 100;
            UpdateThresholdInMilliseconds = 100;

            var app = Application.Current;
        }

        public void SaveActivities()
        {
            ThreadPool.QueueUserWorkItem((async) => InternalSaveActivities());
        }

        private object saveLock = new object();
        private void InternalSaveActivities()
        {
            if (loadLock == 1) return;
            SaveActivitiesToStorage();

        }

        public bool CachedActivitiesLoaded
        {
            get
            {


                // Make sure activities are being loaded
                var activities = Activities;

                // Wait for the load to complete
                activitiesLoaded.WaitOne();
                return true;
            }
        }

        private string[] onDemandSources;
        private bool loadMoreResults;
        private void LoadConfiguration(params string[] onDemandSourcesByName)
        {
            // Wait for activities to be completely loaded
            var loaded = CachedActivitiesLoaded;

            if (onDemandSourcesByName != null && onDemandSourcesByName.Length > 0)
            {
                onDemandSources = onDemandSourcesByName;
            }

            if (Interlocked.CompareExchange(ref loadLock, 1, 0) == 1) return;
            try
            {

                ReaderSettings.Instance.UpdateCompletedOnLaunch = false;
                if (Configuration == null) return;

                if (SourceUrls == null)
                {
                    LoadSourceUrlsAndParsers();
                }



                IsUpdating = true;
                IsUpdatingChanged.SafeRaise(this, EventArgs.Empty);

                var sources = Configuration.Sources;

                if (onDemandSources != null)
                {
                    // Load on demand sources only
                    sources = (from src in sources
                               where onDemandSources.Contains(src.SourceName)
                               select src).ToList();
                    onDemandSources = null;
                }
                else
                {
                    // Only load source that are not marked ondemand
                    sources = sources.Where(src => !src.OnDemand).ToList();
                }


                Func<SocialSource, Stream, IEnumerable<SocialActivity>> activityParser;
                string url;
                foreach (var source in sources)
                {

                    activityParser = Parsers[source.SourceTypeName].Parse;
                    url = SourceUrls[source.SourceTypeName].SourceDownloadUrl(source, loadMoreResults);

                    // Skip any feed that returns a null url
                    if (string.IsNullOrEmpty(url)) continue;

                    var request = HttpWebRequest.Create(url) as HttpWebRequest;
                    request.BeginGetResponse((async) =>
                                                 {
                                                     try
                                                     {
                                                         using (var response = request.EndGetResponse(async))
                                                         using (var strm = response.GetResponseStream())
                                                         {
                                                             var activities = activityParser(source, strm);

                                                             if (activities != null)
                                                             {
                                                                 var current = Activities;
                                                                 if (current == null)
                                                                 {
                                                                     current = new ObservableCollection<ISocialActivityWrapper>();
                                                                 }

                                                                 var currentKeys = (from existing in current
                                                                                    select existing.SocialActivity.Id).ToArray();
                                                                 var newActs = (from act in activities
                                                                                let id = source.SourceName + "_" + act.Id
                                                                                where !currentKeys.Contains(id)
                                                                                select act).ToArray();
                                                                 using (var context = Connect())
                                                                 {
                                                                     foreach (var act in newActs)
                                                                     {
                                                                         act.Id = source.SourceName + "_" + act.Id;
                                                                         act.FeedSourceName = source.SourceName;
                                                                         context.SocialActivities.InsertOnSubmit(act);
                                                                         foreach (var enc in act.Enclosures)
                                                                         {
                                                                             enc.Id = Guid.NewGuid();
                                                                             context.Enclosures.InsertOnSubmit(enc);
                                                                         }
                                                                         context.SubmitChanges();

                                                                         // Force evaluation of enclosures
                                                                         var encs = act.Enclosures.ToArray();
                                                                     }
                                                                 }

                                                                 foreach (var act in newActs)
                                                                 {
                                                                     if (lastupdate > DateTime.Now.AddMilliseconds(-UpdateThresholdInMilliseconds))
                                                                     {
                                                                         Thread.Sleep(UpdateThresholdInMilliseconds);
                                                                     }
                                                                     var wrapper = CreateSocialActivityWrapper(source, act);
                                                                     AddToVisualList(wrapper);
                                                                 }
                                                             }
                                                         }
                                                     }
                                                     catch (Exception ex)
                                                     {
                                                         Debug.WriteLine(ex.Message);
                                                         // The web call fails.... get on with life :-)
                                                     }
                                                     requestEvent.Set();
                                                 }, null);
                    requestEvent.WaitOne();


                }

                // Save activities - this will do a clean up of old activities
                SaveActivitiesToStorage();

                ReaderSettings.Instance.UpdateCompletedOnLaunch = true;
            }
            finally
            {
                IsUpdating = false;
                IsUpdatingChanged.SafeRaise(this, EventArgs.Empty);
                Interlocked.Decrement(ref loadLock);
            }

            if (onDemandSources != null)
            {
                LoadConfiguration();
            }

        }

        private object sourceparser = new object();
        private void LoadSourceUrlsAndParsers()
        {
            lock (sourceparser)
            {
                if (SourceUrls != null) return;

                SourceUrls = new Dictionary<string, ISourceUrl>();
                Parsers = new Dictionary<string, ISourceParser>();

                foreach (var activityType in Configuration.SocialTypes)
                {
                    var parser = Activator.CreateInstance(Type.GetType(activityType.ParserTypeName)) as ISourceParser;
                    Parsers[activityType.SocialTypeName] = parser;
                    var sourceurl = Activator.CreateInstance(Type.GetType(activityType.SourceUrlTypeName)) as ISourceUrl;
                    if (!string.IsNullOrWhiteSpace(activityType.AssociatedSocialProvider))
                    {
                        sourceurl.AssociatedNetwork =
                            Configuration.SocialProviders.FirstOrDefault(
                                sn => sn.ProviderName == activityType.AssociatedSocialProvider);
                    }
                    SourceUrls[activityType.SocialTypeName] = sourceurl;


                    if (!string.IsNullOrWhiteSpace(activityType.Icon))
                    {
                        Configuration.Sources.Where(src => src.SourceTypeName == activityType.SocialTypeName).DoForEach(
                            src => src.SourceImage = activityType.Icon);
                    }
                }

                using (var context = Connect())
                {
                    if (!context.DatabaseExists())
                    {
                        context.CreateDatabase();
                        DatabaseSchemaUpdater dbUpdater = context.CreateDatabaseSchemaUpdater();
                        dbUpdater.DatabaseSchemaVersion = 1;
                        dbUpdater.Execute();
                    }

                    // Upgrading database: Uncomment this code and add changes in place of call to AddTable
                    //else
                    //{
                    //    DatabaseSchemaUpdater dbUpdater = context.CreateDatabaseSchemaUpdater();
                    //    if(dbUpdater.DatabaseSchemaVersion<2)
                    //    {
                    //        dbUpdater.AddTable<>(); // AddColumn, AddIndex, AddAssociation
                    //        dbUpdater.Execute();
                    //    }
                    //}
                }

            }
        }

        private AutoResetEvent VisualUpdateLock = new AutoResetEvent(false);

        private void AddToVisualList(ISocialActivityWrapper activity)
        {
            AddActivityToAllLists(activity);

            SocialFeedLoaded.SafeRaise(this, new ParameterEventArgs<ISocialActivityWrapper>(activity));
        }

        private void AddActivityToAllLists(ISocialActivityWrapper activity)
        {
            AddToSortedCollection(Activities, activity);

            foreach (var filteredActivityList in FilteredLists)
            {
                if (filteredActivityList.IncludeInList(activity))
                {
                    AddToSortedCollection(filteredActivityList.Activities, activity);
                }
            }
        }

        private void FillCollection(IEnumerable<ISocialActivityWrapper> activities, bool uiThread)
        {
            if (activities == null) return;
            foreach (var socialActivity in activities)
            {
                if (!uiThread)
                {
                    AddActivityToAllLists(socialActivity);
                }
                else
                {
                    AddToVisualList(socialActivity);
                }
            }
        }

        protected virtual ISocialActivityWrapper CreateSocialActivityWrapper(SocialSource source, SocialActivity activity)
        {
            var newActivity = new SocialActivityWrapper()
            {
                SocialActivity = activity,
                FeedSource = source,
                FullFunctionality = IsInFullFunctionalityMode,
                LimitedFunctionality = IsInLimitedFunctionalityMode,
                TrialModeOver = IsInTrialExpiredMode
            };

            return newActivity;
        }

        private void AddToSortedCollection(ObservableCollection<ISocialActivityWrapper> collection, ISocialActivityWrapper activity)
        {

            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].SocialActivity.TimeStamp < activity.SocialActivity.TimeStamp)
                {
                    collection.Insert(i, activity);
                    return;
                }
            }
            collection.Add(activity);
        }



        private DateTime lastupdate = DateTime.MinValue;



        //private void SaveActivitiesToStorage(SocialSource source)
        //{
        //    try
        //    {
        //        var activities = (from act in Activities
        //                          where act.FeedSource == source
        //                          select act);
        //        SaveGroupedActivities(activities, source);
        //    }
        //    catch(Exception ex)
        //    {
        //    }
        //}

        //private void SaveGroupedActivities(IEnumerable<ISocialActivityWrapper> group, SocialSource source)
        //{
        //    var items =
        //                        group.OrderByDescending(act => act.SocialActivity.TimeStamp)
        //                            .Take(MaximumActivitiesToCache)
        //                            .Select((act) => act.SocialActivity).ToArray();
        //    ReaderSettings.Instance.SaveActivities(items, source);
        //}

        private void SaveActivitiesToStorage()
        {

            try
            {
                using (var context = Connect())
                {
                    var changed = (from dbact in context.SocialActivities.ToArray()
                                   from act in Activities
                                   where dbact.Id == act.SocialActivity.Id
                                         && dbact.Read != act.SocialActivity.Read
                                   select new { DB = dbact, Current = act }).ToArray();
                    changed.DoForEach(chg => chg.DB.Read = chg.Current.Read);
                    context.SubmitChanges();


                    var grouped = (from dbact in context.SocialActivities
                                   group dbact by dbact.FeedSourceName
                                       into gacts
                                       select gacts);
                    foreach (var group in grouped)
                    {
                        var items =
                            group.OrderByDescending(act => act.TimeStamp)
                                .Skip(MaximumActivitiesToCache).ToArray();
                        items.DoForEach(item => context.SocialActivities.DeleteOnSubmit(item));
                    }
                    context.SubmitChanges();
                }

                //var grouped = (from act in Activities
                //                       group act by act.FeedSource
                //                           into gacts
                //                           select gacts);
                //        foreach (var group in grouped)
                //        {
                //            SaveGroupedActivities(group, group.Key);
                //            //var items =
                //            //    group.OrderByDescending(act => act.SocialActivity.TimeStamp)
                //            //        .Take(MaximumActivitiesToCache)
                //            //        .Select((act)=>act.SocialActivity).ToArray();
                //            //ReaderSettings.Instance.SaveActivities(items, group.Key, SocialActivityType);
                //        }

                ////var serializer = new DataContractJsonSerializer(typeof(SocialActivity[]));
                ////using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                ////{
                ////    //if (store.FileExists(ActivitiesFileName))
                ////    //{
                ////    //    store.DeleteFile(ActivitiesFileName);
                ////    //}

                ////    using (
                ////        var file = store.OpenFile(ActivitiesFileName, FileMode.Create, FileAccess.Write, FileShare.None)
                ////        )
                ////    {
                ////        var grouped = (from act in Activities
                ////                       group act by act.SocialActivity.FeedSource
                ////                           into gacts
                ////                           select gacts);

                ////        var ungrouped = (from gact in grouped
                ////                         from act in
                ////                             gact.OrderByDescending(act => act.SocialActivity.TimeStamp).Take(MaximumActivitiesToCache)
                ////                         select act.SocialActivity).OrderByDescending(act => act.TimeStamp).ToList();


                ////        var activitiesToSave = ungrouped;
                ////        //Activities.OrderByDescending(act => act.TimeStamp).Take(MaximumActivitiesToCache).ToList();
                ////        serializer.WriteObject(file, activitiesToSave.ToArray());
                ////    }
                ////}
            }
            catch
            {
            }
        }

        private ManualResetEvent activitiesLoaded = new ManualResetEvent(false);
        private void RestoreActivitiesFromStorage()
        {
            try
            {
                if (SourceUrls == null)
                {
                    LoadSourceUrlsAndParsers();
                }

                if (!listsCreated)
                {
                    Configuration.Lists.CreateActivityLists(this);
                    listsCreated = true;
                }

                if (!DesignerProperties.IsInDesignTool)
                {

                    ThreadPool.QueueUserWorkItem((async) =>
                                                     {
                                                         IsLoading = true;
                                                         IsLoadingChanged.SafeRaise(this, EventArgs.Empty);

                                                         var sourceDict = new Dictionary<string, SocialSource>();
                                                         foreach (var socialSource in Configuration.Sources)
                                                         {
                                                             sourceDict[socialSource.SourceName] = socialSource;
                                                         }

                                                         using (var context = Connect())
                                                         {
                                                             var loadedActs = (from act in context.SocialActivities
                                                                               let enc = act.Enclosures.ToArray()
                                                                               let source =
                                                                                   sourceDict[act.FeedSourceName]
                                                                               select
                                                                                   CreateSocialActivityWrapper(source,
                                                                                                               act))
                                                                 .ToArray();
                                                             FillCollection(loadedActs, true);
                                                         }

                                                         IsLoading = false;
                                                         IsLoadingChanged.SafeRaise(this, EventArgs.Empty);

                                                         activitiesLoaded.Set();

                                                         if (PhoneApplicationService.Current.StartupMode ==
                                                             StartupMode.Launch ||
                                                             ReaderSettings.Instance.UpdateCompletedOnLaunch == false)
                                                         {
                                                             LoadConfiguration();
                                                         }
                                                     });
                }
                // Create the activities collection and populate it.
                activities = new ObservableCollection<ISocialActivityWrapper>();

            }
            catch
            {
            }
            if (activities == null)
            {
                activities = new ObservableCollection<ISocialActivityWrapper>();
            }
        }




        public PageAdInformation PageAd(Type pageType)
        {
            var def = Configuration.Structure.FindDefinitionByType(pageType);
            if (string.IsNullOrWhiteSpace(def.AdProviderName) || string.IsNullOrWhiteSpace(def.AdUnitName)) return new PageAdInformation();
            var provider = (from prov in Configuration.AdProviders
                            where prov.AdProviderName == def.AdProviderName
                            select prov).FirstOrDefault();
            if (provider == null) return new PageAdInformation();
            var unit = (from u in provider.AdUnits
                        where u.AdUnitName == def.AdUnitName
                        select u).FirstOrDefault();
            return new PageAdInformation()
                       {
                           Provider = provider,
                           Unit = unit
                       };
        }

        //public Visibility AdsVisibility
        //{
        //    get
        //    {
        //        return (!string.IsNullOrEmpty(Configuration.AdPublisherId)).ToVisibility();
        //    }
        //}

        private DateTime firstRun;
        public DateTime FirstRun
        {
            get { return firstRun; }
        }

        public DateTime TrialExpiresOn
        {
            get
            {
                if (Configuration.Testing != TestModeOptions.Disabled)
                {
                    switch (configuration.Testing)
                    {
                        case TestModeOptions.Full:
                            return DateTime.MaxValue;
                        case TestModeOptions.Limited:
                            return DateTime.Now.AddDays(+1);
                        case TestModeOptions.TrialExpired:
                            return DateTime.Now.AddDays(-1);
                    }
                }

                if (Configuration.TrialModeLengthInDays <= 0) return DateTime.MaxValue;
                return FirstRun.AddDays(Configuration.TrialModeLengthInDays);
            }
        }

        public DateTime FullFunctionalityTrialExpiresOn
        {
            get
            {
                if (Configuration.Testing != TestModeOptions.Disabled)
                {
                    switch (configuration.Testing)
                    {
                        case TestModeOptions.Full:
                            return DateTime.MaxValue;
                        case TestModeOptions.Limited:
                            return DateTime.Now.AddDays(-1);
                        case TestModeOptions.TrialExpired:
                            return DateTime.Now.AddDays(-2);
                    }
                }


                if (!Configuration.FullFunctionalityInTrialMode) return DateTime.MinValue;
                if (Configuration.FullFunctionalityTrialModeLengthInDays <= 0) return TrialExpiresOn;
                return FirstRun.AddDays(Configuration.FullFunctionalityTrialModeLengthInDays);
            }
        }


        public bool IsInFullFunctionalityTrialPeriod
        {
            get
            {
                return DateTime.Now.Date < FullFunctionalityTrialExpiresOn.Date;
            }
        }

        public bool IsInLimitedFunctionalityTrialPeriod
        {
            get
            {
                return DateTime.Now.Date >= FullFunctionalityTrialExpiresOn && DateTime.Now.Date < TrialExpiresOn.Date;
            }
        }

        public bool IsInTrialPeriod
        {
            get
            {
                return DateTime.Now.Date < TrialExpiresOn.Date;
            }
        }

        public bool IsInTrialMode
        {
            get { return (Configuration.Testing != TestModeOptions.Disabled || Utilities.IsInTrialMode(null)); }
        }

        public bool IsInFullFunctionalityMode
        {
            get
            {
                return (!IsInTrialMode || IsInFullFunctionalityTrialPeriod);
            }
        }

        public bool IsInLimitedFunctionalityMode
        {
            get { return IsInTrialMode && IsInLimitedFunctionalityTrialPeriod; }
        }
        public bool IsInTrialExpiredMode
        {
            get { return IsInTrialMode && !IsInTrialPeriod; }
        }

        public Visibility FullFunctionalityVisibility
        {
            get { return IsInFullFunctionalityMode.ToVisibility(); }
        }

        public Visibility LimitedFunctionalityVisibility
        {
            get { return IsInLimitedFunctionalityMode.ToVisibility(); }
        }

        public Visibility TrialPeriodOverVisibility
        {
            get { return IsInTrialExpiredMode.ToVisibility(); }
        }

        private ISocial urlShortener;

        public ISocial UrlShortener
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Configuration.LinkShortener)) return null;
                if (urlShortener == null)
                {
                    urlShortener = (from s in Configuration.SocialProviders
                                    where s.ProviderName == Configuration.LinkShortener
                                    select s.Instance).FirstOrDefault();
                }
                return urlShortener;
            }
        }

        public bool IsShortenerAvailable
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Configuration.LinkShortener)) return false;
                return UrlShortener != null;
            }
        }

        private string ListTileUri(string listName)
        {
            return string.Format("/tile?listname={0}", HttpUtility.UrlEncode(listName));
        }

        public bool IsPinned(string listName)
        {
            var tileUri = ListTileUri(listName);
            var existingTile = (from tile in ShellTile.ActiveTiles
                                where tile.NavigationUri.OriginalString == tileUri
                                select tile).FirstOrDefault();
            return existingTile != null;
        }


        public void PinActivityListToStart(string listName)
        {
            if (string.IsNullOrWhiteSpace(listName)) return;
            var tileUri = ListTileUri(listName);
            var existingTile = (from tile in ShellTile.ActiveTiles
                                where tile.NavigationUri.OriginalString == tileUri
                                select tile).FirstOrDefault();
            if (existingTile != null) return;

            var tileData = new StandardTileData()
                               {
                                   Title = listName,
                                   BackTitle = "WINDOWS PHONE NEWS"
                               };
            ShellTile.Create(new Uri(tileUri, UriKind.Relative), tileData);
        }
        public void UnPinActivityListToStart(string listName)
        {
            if (string.IsNullOrWhiteSpace(listName)) return;
            var tileUri = ListTileUri(listName);
            var existingTile = (from tile in ShellTile.ActiveTiles
                                where tile.NavigationUri.OriginalString == tileUri
                                select tile).FirstOrDefault();
            if (existingTile == null) return;
            existingTile.Delete();

        }
    }



}