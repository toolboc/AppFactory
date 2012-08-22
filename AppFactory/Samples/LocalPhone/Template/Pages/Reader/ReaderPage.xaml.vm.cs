using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using BuiltToRoam;
using BuiltToRoam.Interfaces;
using BuiltToRoam.WindowsPhone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Template.Config.Content.Lists;
using Template.Config.Implementations;
using Template.Config.Structure;
using Template.Data;
using Template.Pages.Main;
using Template.Pages.Post;

namespace Template.Pages.Reader
{
    public enum ReaderPageStates
    {
        Base,
        LinksHidden,
        LinksVisible
    }

    public enum ReaderAppBar
    {
        comment,
        repost,
        post,
        reply,
        reply_all,
        email,
        previous,
        next,
        links
    }

    public class ReaderPageViewModel : SocialReaderViewModelBase<ReaderPageStates, ReaderAppBar>
    {
        /// <summary>
        /// The query string keys
        /// </summary>
        public const string ActivitySourceKey = "source";
        public const string ActivityIdKey = "id";
        public const string ActvitiyListKey = "list";

        /// <summary>
        /// The links available for the current activity
        /// </summary>
        private List<Link> links;

        /// <summary>
        /// Whether the links list is visible
        /// </summary>
        private Visibility linksVisibility = Visibility.Collapsed;

        /// <summary>
        /// The current activity
        /// </summary>
        private ISocialActivityWrapper activity;

        /// <summary>
        /// The list of activities (for previous/next)
        /// </summary>
        private ObservableCollection<ISocialActivityWrapper> activityList;

        /// <summary>
        /// The index of the current activity in the activity list
        /// </summary>
        private int activityIndex;

        /// <summary>
        /// The name of the feed source (from navigation)
        /// </summary>
        private string navigationSource;

        /// <summary>
        /// The id of the activity to display  (from navigation)
        /// </summary>
        private string navigationId;

        /// <summary>
        /// The list index of the activities  (from navigation)
        /// </summary>
        private int navigationListIndex;

        /// <summary>
        /// Which activites in the list have been read
        /// </summary>
        private int[] readIndexes;

        /// <summary>
        /// The activity which is persisted during tombstoning
        /// </summary>
        private ISocialActivityWrapper navigationActivity;


        private PageImplementation implementation;

        public PageImplementation Implementation
        {
            get { return implementation; }
            set
            {
                if (Implementation == value) return;
                implementation = value;
                RaisePropertyChanged(() => Implementation);
            }
        }



        /// <summary>
        /// Initializes a new instance of the ReaderPageViewModel
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dispatcher"></param>
        /// <param name="navigation"></param>
        public ReaderPageViewModel(IRepository repository, IDispatcher dispatcher, INavigation navigation)
            : base(repository, dispatcher, navigation)
        {

        }

        /// <summary>
        /// Gets whether the ads control should be visible
        /// </summary>
        public override PageAdInformation PageAd
        {
            get
            {
                var pageAd = base.PageAd;

                if (!(Repository.IsInLimitedFunctionalityTrialPeriod))
                {
                    pageAd.Provider = null;
                    pageAd.Unit = null;
                }

                return pageAd;
            }
        }

        /// <summary>
        /// Gets/Sets the list of links for the current activity
        /// </summary>
        public List<Link> Links
        {
            get { return links; }
            set
            {
                if (IsInDesignMode) return;
                links = value;
                RaisePropertyChanged(() => Links);

                SetupApplicationBar();
            }
        }

        /// <summary>
        /// Gets/Sets the current activity
        /// </summary>
        public ISocialActivityWrapper Activity
        {
            get { return activity; }
            set
            {
                if (Activity == value) return;
                activity = value;
                RaisePropertyChanged(() => Activity);
                RaisePropertyChanged(() => ActivityTitleVisibility);
                if (Activity != null)
                {
                    var def = Definition as ReaderPageDefinition;
                    Implementation = new ReaderPageImplementation(Repository, def, Activity);

                    Activity.SocialActivity.Read = true;
                    if (ApplicationBarVisibility == Visibility.Visible)
                    {
                        // Need to update the application bar icons
                        SetupApplicationBar();
                    }

                    ShortenActivityUrl();
                }
            }
        }

        /// <summary>
        /// Gets/Sets the list of activities
        /// </summary>
        public ObservableCollection<ISocialActivityWrapper> ActivityList
        {
            get { return activityList; }
            set
            {
                if (ActivityList == value) return;
                activityList = value;

                // Refresh the read status (as this isn't persisted until the user
                // goes back to the main page of the application
                if (ActivityList != null && readIndexes != null)
                {
                    foreach (var index in readIndexes)
                    {
                        if (index >= 0 && index < ActivityList.Count)
                        {
                            ActivityList[index].SocialActivity.Read = true;
                        }
                    }
                }
                RaisePropertyChanged(() => ActivityList);
            }
        }

        /// <summary>
        /// Gets/Sets the activity index
        /// </summary>
        public int ActivityIndex
        {
            get { return activityIndex; }
            set
            {
                if (ActivityIndex == value) return;
                activityIndex = value;
                RaisePropertyChanged(() => ActivityIndex);
            }
        }

        /// <summary>
        /// Gets whether the activity title should be visible
        /// </summary>
        public Visibility ActivityTitleVisibility
        {
            get
            {
                return (Activity != null && !string.IsNullOrWhiteSpace(Activity.SocialActivity.Title)).ToVisibility();
            }
        }


        /// <summary>
        /// Handles the navigated event so as to record the activity data
        /// </summary>
        public override void OnNavigatedTo(NavigationEventArgs navigationEventArgs, NavigationContext context, IDictionary<string, object> state, bool firstLoad)
        {
            base.OnNavigatedTo(navigationEventArgs, context, state, firstLoad);

            var query = context.QueryString;

            ChangePageState(ReaderPageStates.LinksHidden, false);

            try
            {
                if (string.IsNullOrEmpty(navigationSource))
                {
                    navigationSource = query[ActivitySourceKey];
                    navigationId = query[ActivityIdKey];
                    navigationListIndex = int.Parse(query[ActvitiyListKey]);
                }
            }
            catch
            {
                // If there was an error, just return to the main page
                GoBack();
            }
        }

        public override void OnNavigatingFrom(NavigatingCancelEventArgs cancelArgs, IDictionary<string, object> state)
        {
            base.OnNavigatingFrom(cancelArgs, state);

            if (!cancelArgs.Cancel)
            {
                ApplicationBarVisibility = Visibility.Collapsed;


                // If going back to main page - save the read state of the activites
                if (cancelArgs.NavigationMode == NavigationMode.Back)
                {
                    Repository.SaveActivities();
                }
            }
        }

        /// <summary>
        /// Handles loading data (background thread)
        /// </summary>
        /// <param name="isFirstLoad"></param>
        public override void LoadViewModelData(bool isFirstLoad)
        {
            base.LoadViewModelData(isFirstLoad);

            // Make sure the cached activities are all loaded
            // WARNING: This is a blocking property call so 
            // should only be done on a background thread
            var loaded = Repository.CachedActivitiesLoaded;
            Console.WriteLine(loaded);
            if (isFirstLoad)
            {

                // Find the activities list
                ActivityList = Repository.Activities;
                if (navigationListIndex >= 0)
                {
                    ActivityList = Repository.FilteredLists[navigationListIndex].Activities;
                }

                // Load the activity by index
                var idx = -1;
                var acts = (from act in ActivityList
                            let newIndex = idx++
                            where act.SocialActivity.FeedSourceName == navigationSource &&
                                  act.SocialActivity.Id == navigationId
                            select act).FirstOrDefault();
                ActivityIndex = idx;
                if (acts != null)
                {
                    navigationActivity = acts;
                }
            }

        }


        /// <summary>
        /// Handles when the view model is completely loaded
        /// </summary>
        /// <param name="isFirstLoad"></param>
        public override void ViewModelLoadComplete(bool isFirstLoad)
        {
            base.ViewModelLoadComplete(isFirstLoad);

            // Hide all the app bar items
            HideAllApplicationBarItems();

            // Configure the app bar items
            if (Activity != null)
            {
                SetupApplicationBar();
            }

            // Display the app bar
            ShowApplicationBar();

            Activity = navigationActivity;
        }

        /// <summary>
        /// Display the application bar
        /// </summary>
        protected void ShowApplicationBar()
        {
            // Only show the application bar if running in full mode
            ApplicationBarVisibility = Repository.FullFunctionalityVisibility;
        }

        /// <summary>
        /// Hide all of the application bar items by removing them
        /// </summary>
        private void HideAllApplicationBarItems()
        {
            HideApplicationBarItem(ReaderAppBar.comment);
            HideApplicationBarItem(ReaderAppBar.post);
            HideApplicationBarItem(ReaderAppBar.repost);
            HideApplicationBarItem(ReaderAppBar.reply);
            HideApplicationBarItem(ReaderAppBar.reply_all);

            HideApplicationBarItem(ReaderAppBar.email);
            HideApplicationBarItem(ReaderAppBar.previous);
            HideApplicationBarItem(ReaderAppBar.next);
            HideApplicationBarItem(ReaderAppBar.links);
        }


        /// <summary>
        /// Setup the application bar depending on the type of item
        /// </summary>
        private void SetupApplicationBar()
        {
            if (Activity == null || ActivityList == null) return;

            var canpost = Configuration.SocialProviders.Any(sp => sp.Instance.CanPost);
            ShowHideApplicationBarItem(ReaderAppBar.post, canpost);

            var canrepost = Configuration.SocialProviders.Any(sp => sp.Instance.CanRepost);
            ShowHideApplicationBarItem(ReaderAppBar.repost, canrepost);

            var cancomment = Configuration.SocialProviders.Any(sp => sp.Instance.CanComment);
            ShowHideApplicationBarItem(ReaderAppBar.comment, cancomment);

            ShowApplicationBarItem(ReaderAppBar.email);

            ApplicationBarItemIsEnabled(ReaderAppBar.previous, ActivityIndex > 0);
            ApplicationBarItemIsEnabled(ReaderAppBar.links, Links != null && Links.Count > 0);
            ApplicationBarItemIsEnabled(ReaderAppBar.next, ActivityIndex < ActivityList.Count - 1);
            ShowApplicationBarItem(ReaderAppBar.previous);
            ShowApplicationBarItem(ReaderAppBar.links);
            ShowApplicationBarItem(ReaderAppBar.next);
        }

        /// <summary>
        /// Shorten the link to the current activity
        /// </summary>
        private void ShortenActivityUrl()
        {
            // Check to make sure BitLy has been enabled
            if (!Repository.IsShortenerAvailable) return;

            if (Activity != null && !Activity.HasShortenedUrl)
            {
                Repository.UrlShortener.ShortenUrlCompleted += ShortenCallback;
                Repository.UrlShortener.ShortenUrl(Activity.SocialActivity.Url);
            }

        }

        /// <summary>
        /// Callback for shortening the url for the current activity
        /// </summary>
        private void ShortenCallback(object sender, TripleParameterEventArgs<bool, string, Exception> e)
        {
            Repository.UrlShortener.ShortenUrlCompleted -= ShortenCallback;
            if (e.Parameter1)
            {
                Activity.SocialActivity.ShortenedUrl = e.Parameter2;
            }
        }

        private void PostMessage(PostPageViewModel.PostType typeOfPost, string message, string title)
        {
            PhoneApplicationService.Current.State[PostPageViewModel.PostTypeKey] = typeOfPost;
            PhoneApplicationService.Current.State[PostPageViewModel.PostMessageKey] = message;
            PhoneApplicationService.Current.State[PostPageViewModel.PostTitleKey] = title;

            Navigation.Navigate(typeof(PostPage), new[]
                                                      {
            NavigationParameter<string,string>.StringParameter(ActivityIdKey, Activity.SocialActivity.Id)
                                                      });
        }

        /// <summary>
        /// Get ready to post
        /// </summary>
        public void Post()
        {
            PostMessage(PostPageViewModel.PostType.Post,
                Activity.SocialActivity.FeedSourceName + " post (" + Activity.SocialActivity.ShortenedUrl + ")",
                "POST A MESSAGE"
                );
        }

        /// <summary>
        /// Get ready to repost
        /// </summary>
        public void Repost()
        {
            PostMessage(PostPageViewModel.PostType.Repost,
                Activity.SocialActivity.Description,
                "REPOST A MESSAGE"
                );
        }

        /// <summary>
        /// Get ready to post a comment
        /// </summary>
        public void Comment()
        {
            PostMessage(PostPageViewModel.PostType.Comment,
                string.Empty,
                "POST A COMMENT"
                );
        }

        /// <summary>
        /// Get ready to reply
        /// </summary>
        public void Reply()
        {
            PostMessage(PostPageViewModel.PostType.Reply,
                Activity.SocialActivity.Author,
                "POST A REPLY"
                );
        }

        /// <summary>
        /// Get ready to reply all
        /// </summary>
        public void ReplyAll()
        {
            PostMessage(PostPageViewModel.PostType.ReplyAll,
                Authors,
                "POST A REPLY TO ALL"
                );
        }

        /// <summary>
        /// Extract the list of authors from the current activity
        /// </summary>
        public string Authors
        {
            get
            {
                var authorList = new StringBuilder();
                var post = Activity.SocialActivity.Description;
                int idx = 0;
                var authors = new List<string>();
                if (!string.IsNullOrWhiteSpace(post))
                {
                    while (idx < post.Length && (idx = post.IndexOf("@", idx)) >= 0)
                    {
                        var eidx = post.IndexOf(" ", idx);
                        authors.Add(eidx >= 0
                                        ? post.Substring(idx, eidx - idx).ToLower()
                                        : post.Substring(idx).ToLower());
                        idx++;
                    }
                }

                if (!string.IsNullOrEmpty(Activity.SocialActivity.Author))
                {
                    authors.Add(Activity.SocialActivity.Author.ToLower());
                }

                authors.Distinct().DoForEach(author =>
                                                 {
                                                     if (authorList.Length >= 0) authorList.Append(" ");
                                                     authorList.Append(author);
                                                 });
                return authorList.ToString();
            }
        }


        /// <summary>
        /// Generate an email
        /// </summary>
        public void Email()
        {
            var task = new EmailComposeTask
                           {
                               Subject =
                                   "Shared: " +
                                   (!string.IsNullOrEmpty(Activity.SocialActivity.Title)
                                        ? ": " + Activity.SocialActivity.Title
                                        : ""),
                               Body =
                                   Activity.SocialActivity.FeedSourceName + " post (" + Activity.SocialActivity.ShortenedUrl +
                                   ")\r\n\r\n"
                           };
            task.Show();
        }

        public override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            // If the links list is visible, close it and cancel
            if (CurrentState<ReaderPageStates>() == ReaderPageStates.LinksVisible)
            {
                e.Cancel = true;
                HideLinkList();
            }
        }

        /// <summary>
        /// Open the author in the webbrowser
        /// </summary>
        public void OpenAuthor()
        {
            if (Activity.SocialActivity.Author.StartsWith("@"))
            {
                Utilities.DisplayInWebBrowser("http://twitter.com/" + Activity.SocialActivity.Author);
            }
        }

        /// <summary>
        /// Open the activity in the web browser
        /// </summary>
        public void OpenInBrowser()
        {
            Utilities.DisplayInWebBrowser(Activity.SocialActivity.Url);
        }

        /// <summary>
        /// Go to the previous activity
        /// </summary>
        public void Previous()
        {
            ActivityIndex--;
            if (ActivityIndex < 0) ActivityIndex = 0;
            if (ActivityList == null || ActivityIndex >= ActivityList.Count) return;
            Activity = ActivityList[ActivityIndex];
        }

        /// <summary>
        /// Go to the next activity
        /// </summary>
        public void Next()
        {
            ActivityIndex++;
            if (ActivityList == null || ActivityIndex >= ActivityList.Count) return;
            Activity = ActivityList[ActivityIndex];
        }

        /// <summary>
        /// Show the list of links
        /// </summary>
        public void ShowLinkList()
        {
            ChangePageState(ReaderPageStates.LinksVisible);
            ApplicationBarVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Hide the list of links
        /// </summary>
        public void HideLinkList()
        {
            ChangePageState(ReaderPageStates.LinksHidden);
            ApplicationBarVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Display the selected link
        /// </summary>
        /// <param name="link"></param>
        public void DisplayLink(Link link)
        {
            HideLinkList();
            Utilities.DisplayInWebBrowser(link.Url);
        }

        /// <summary>
        /// Persist transient information (ie handle tombstoning)
        /// </summary>
        /// <param name="dictionary"></param>
        public void Save(IDictionary<string, object> dictionary)
        {
            // Activity can be null if the user rapidly presses the back button - since
            // it takes a couple of seconds to load the repository into memory
            if (Activity != null && ActivityList != null)
            {
                dictionary[ActivitySourceKey] = Activity.SocialActivity.FeedSourceName;
                dictionary[ActvitiyListKey] = navigationListIndex;
                dictionary[ActivityIdKey] = Activity.SocialActivity.Id;
                int idx = 0;
                dictionary["read"] = (from act in ActivityList
                                      let actidx = idx++
                                      where act.SocialActivity.Read
                                      select actidx).ToArray();
                dictionary["activityType"] = Activity.SocialActivity.GetType().AssemblyQualifiedName;
                dictionary["wrapperType"] = Activity.GetType().AssemblyQualifiedName;
                var serializer = new DataContractJsonSerializer(Activity.GetType(), new[] { Activity.SocialActivity.GetType() });
                using (var ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, Activity);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    dictionary["activity"] = ms.ToArray();
                }


            }
        }

        /// <summary>
        /// Restore persisted transient information
        /// </summary>
        /// <param name="dictionary"></param>
        public void Restore(IDictionary<string, object> dictionary)
        {
            navigationSource = dictionary.SafeDictionaryValue<string, object, string>(ActivitySourceKey);
            navigationListIndex = dictionary.SafeDictionaryValue<string, object, int>(ActvitiyListKey);
            navigationId = dictionary.SafeDictionaryValue<string, object, string>(ActivityIdKey);
            readIndexes = dictionary.SafeDictionaryValue<string, object, int[]>("read");
            var activityBytes = dictionary.SafeDictionaryValue<string, object, byte[]>("activity");
            if (activityBytes != null && activityBytes.Length > 0)
            {
                var typeName = dictionary.SafeDictionaryValue<string, object, string>("activityType");
                var activitytype = Type.GetType(typeName, false);
                typeName = dictionary.SafeDictionaryValue<string, object, string>("wrapperType");
                var wrapperType = Type.GetType(typeName, false);
                if (activitytype == null || wrapperType == null) return;
                var serializer = new DataContractJsonSerializer(wrapperType, new[] { activitytype });
                using (var ms = new MemoryStream(activityBytes))
                {
                    activity = serializer.ReadObject(ms) as ISocialActivityWrapper;
                    Dispatcher.BeginInvoke(() => RaisePropertyChanged(() => Activity));
                }
            }
        }

        /// <summary>
        /// Set any current states
        /// </summary>
        /// <param name="controlStates"></param>
        public void CurrentStates(IDictionary<string, string> controlStates)
        {
        }
    }
}
