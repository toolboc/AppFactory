using InfoHub.Articles;
using InfoHub.Feeds;
using InfoHub.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InfoHub
{
    public partial class AppHubViewModel : INotifyPropertyChanged
    {
        #region Format

        void AddFeeds(bool fake)
        {
            // adds feeds to the app (the order here equals the order in the app)

            if (fake)
            {
                // these are fake feeds
                // these are for designtime 

                //if (Strings.SummaryInclude) this.Feeds.Add(new GroupedItem(new FeedBase { Title = Strings.SummaryTitle }) { Articles = new ObservableCollection<IArticle>(Enumerable.Range(0, 0).Select(x => new SummaryArticle { ColSpan = 3, RowSpan = 4 } as IArticle)) });
                if (Strings.NewsInclude) this.Feeds.Add(new GroupedItem(new FeedBase { Title = Strings.NewsTitle, MoreUrl = "http://bing.com" }) { Articles = new ObservableCollection<IArticle>(Enumerable.Range(0, 15).Select(x => new NewsArticle { Index = x, Title = "News Title", Body = "Now is the time for all good men to come to the aid of their country" })) });
                if (Strings.YouTubeInclude) this.Feeds.Add(new GroupedItem(new FeedBase { Title = Strings.YouTubeTitle, MoreUrl = "http://bing.com" }) { Articles = new ObservableCollection<IArticle>(Enumerable.Range(0, 15).Select(x => new YouTubeArticle { Index = x, Image = "ms-appx:///Assets/AltAdvert.jpg", Title = "YouTube Video Title" })) });
                if (Strings.TwitterInclude) this.Feeds.Add(new GroupedItem(new FeedBase { Title = Strings.TwitterTitle, MoreUrl = "http://bing.com" }) { Articles = new ObservableCollection<IArticle>(Enumerable.Range(0, 15).Select(x => new TwitterArticle { Index = x, Body = "Now is the time for all good men to come to the aid of their country" })) });
                if (Strings.FlickrInclude) this.Feeds.Add(new GroupedItem(new FeedBase { Title = Strings.FlickrTitle, MoreUrl = "http://bing.com" }) { Articles = new ObservableCollection<IArticle>(Enumerable.Range(0, 15).Select(x => new FlickrArticle { Index = x, Image = "ms-appx:///Assets/AltAdvert.jpg", Title = "Photo Title" })) });
                if (Strings.CalendarInclude) this.Feeds.Add(new GroupedItem(new FeedBase { Title = Strings.CalendarTitle, MoreUrl = "http://bing.com" }) { Articles = new ObservableCollection<IArticle>(Enumerable.Range(0, 15).Select(x => new CalendarArticle { Index = x, Title = "Calendar Title", Date = DateTime.Now, })) });
                if (Strings.WeatherInclude) this.Feeds.Add(new GroupedItem(new FeedBase { Title = Strings.WeatherTitle, MoreUrl = "http://bing.com" }) { Articles = new ObservableCollection<IArticle>(Enumerable.Range(0, 15).Select(x => new WeatherArticle { Index = x, Image = "http://forecast.weather.gov/images/wtf/medium/nbkn.png", Title = "Mostly Cloudy", Date = DateTime.Now, Body = "65/83F 20%" })) });

                // update them so they look right @ designtime
                foreach (var item in this.Feeds.SelectMany(x => x.Articles))
                    FormatItem(item);
                foreach (var item in this.Feeds)
                    FormatItems(item.Articles);
            }
            else
            {
                // these are real feeds
                // these are for runtime 

                //if (Strings.SummaryInclude) this.Feeds.Add(new GroupedItem(new FeedBase { Title = Strings.SummaryTitle, MoreUrl = Strings.SummaryMoreUrl, }));
                if (Strings.NewsInclude) this.Feeds.Add(new GroupedItem(new RssFeed<NewsArticle> { Title = Strings.NewsTitle, SourceUrl = Strings.NewsSourceUrl, MoreUrl = Strings.NewsMoreUrl, }));
                if (Strings.YouTubeInclude) this.Feeds.Add(new GroupedItem(new RssFeed<YouTubeArticle> { Title = Strings.YouTubeTitle, SourceUrl = Strings.YouTubeSourceUrl, MoreUrl = Strings.YouTubeMoreUrl, }));
                if (Strings.TwitterInclude) this.Feeds.Add(new GroupedItem(new RssFeed<TwitterArticle> { Title = Strings.TwitterTitle, SourceUrl = Strings.TwitterSourceUrl, MoreUrl = Strings.TwitterMoreUrl }));
                if (Strings.FlickrInclude) this.Feeds.Add(new GroupedItem(new RssFeed<FlickrArticle> { Title = Strings.FlickrTitle, SourceUrl = Strings.FlickrSourceUrl, MoreUrl = Strings.FlickrMoreUrl, }));
                if (Strings.CalendarInclude) this.Feeds.Add(new GroupedItem(new CalendarFeed { Title = Strings.CalendarTitle, SourceUrl = Strings.CalendarSourceUrl, MoreUrl = Strings.CalendarMoreUrl, }));
                if (Strings.WeatherInclude) this.Feeds.Add(new GroupedItem(new WeatherFeed
                {
                    Title = Strings.WeatherTitle,
                    SourceUrl = string.Format("http://forecast.weather.gov/MapClick.php?lat={0}&lon={1}&unit=0&lg=english&FcstType=dwml", Strings.WeatherLatitude, Strings.WeatherLongitude),
                    MoreUrl = string.Format("http://forecast.weather.gov/MapClick.php?lat={0}&lon={1}", Strings.WeatherLatitude, Strings.WeatherLongitude),
                }));

                // summary is not a real feed, it needs one hardcoded article
                //this.Feeds.First().Articles.Add(FormatItem(new SummaryArticle() { Data = Strings.SummaryData }));

            }
        }

        IArticle FormatItem(IArticle article)
        {
            // update each article in the grid
            // usually just to resize the article's height and width

            if (article is CalendarArticle)
            {
                // calendar articles are always the same size
                article.ColSpan = 375;
                article.RowSpan = 125;
            }
            else if (article is FlickrArticle)
            {
                // flickr articles have a hero that is larger than the rest
                article.ColSpan = (article.Hero) ? 500 : 250;
                article.RowSpan = (article.Hero) ? 500 : 250;
            }
            else if (article is NewsArticle)
            {
                // news articles have a hero that is larger than the rest
                article.ColSpan = (article.Hero) ? 500 : 250;
                article.RowSpan = (article.Hero) ? 250 : 250;
            }
            else if (article is SummaryArticle)
            {
                // summary article(s) is always the same size, sinc there is only one
                article.ColSpan = 375;
                article.RowSpan = 500;
            }
            else if (article is TwitterArticle)
            {
                // twitter articles are the same size, but are taller if there is more text
                article.ColSpan = 250;
                article.RowSpan = (article.Body.Length > 75) ? 250 : 125;
            }
            else if (article is WeatherArticle)
            {
                // weather articles are all the same size
                article.ColSpan = 125;
                article.RowSpan = 500;
            }
            else if (article is YouTubeArticle)
            {
                // youtube articles have a hero that is larger than the rest
                article.ColSpan = (article.Hero) ? 500 : 250;
                article.RowSpan = (article.Hero) ? 500 : 250;
            }
            return article;
        }

        void FormatItems(ObservableCollection<IArticle> articles)
        {
            // update each article group in the grid
            // usually to add advertisements & trim extras from the list

            int _MaxArticles = default(int);
            if (!articles.Any()) return;
            else if (articles.First() is CalendarArticle)
            {
                _MaxArticles = Strings.CalendarCount;

                // calendar articles are sorted descending
                var _Reverse = articles
                    .Take(_MaxArticles).Reverse().ToArray();
                articles.Clear();
                foreach (var _Item in _Reverse)
                    articles.Add(_Item);
            }
            else if (articles.First() is FlickrArticle)
            {
                _MaxArticles = Strings.FlickrCount;

                // flickr articles get some advertising added in
                if (articles.Count > 3 && !HideAds && !articles.OfType<AdvertArticle>().Any())
                    articles.Insert(2, new AdvertArticle { ColSpan = 250, RowSpan = 250 });
            }
            else if (articles.First() is NewsArticle)
            {
                _MaxArticles = Strings.NewsCount;

                // news articles update the live tile
                SetTileBadge(articles.First().Date.Day);
                SetTileNotification(articles.First().Title);

                // news articles get some advertising added in
                if (articles.Count > 4 && !HideAds && !articles.OfType<AdvertArticle>().Any())
                    articles.Insert(3, new AdvertArticle { ColSpan = 250, RowSpan = 250 });
            }
            else if (articles.First() is SummaryArticle)
            {
                // there can be only one summary article
                _MaxArticles = 1;
            }
            else if (articles.First() is TwitterArticle)
            {
                // twitter article list is left alone
                _MaxArticles = Strings.TwitterCount;
            }
            else if (articles.First() is WeatherArticle)
            {
                _MaxArticles = Strings.WeatherCount;

                // weather articles are sorted descending
                var _Reverse = articles
                    .Take(_MaxArticles).Reverse().ToArray();
                articles.Clear();
                foreach (var _Item in _Reverse)
                    articles.Add(_Item);
            }
            else if (articles.First() is YouTubeArticle)
            {
                _MaxArticles = Strings.YouTubeCount;

                // youtube articles get some advertising added in
                if (articles.Count > 3 && !HideAds && !articles.OfType<AdvertArticle>().Any())
                    articles.Insert(2, new AdvertArticle { ColSpan = 2, RowSpan = 2 });
            }

            // now remove the overflow from the lists
            foreach (var _Item in articles.Skip(_MaxArticles).ToArray())
                articles.Remove(_Item);
        }

        #endregion

        public AppHubViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                AddFeeds(true);

            if (Strings.PrivacyPolicyUseLocal)
            {
                // local privacy statement
                var _Helper = new InfoHub.Helpers.SettingsHelper();
                _Helper.AddCommand<InfoHub.Privacy>(Strings.PrivacyPolicyText);
            }
            else
            {
                // remote privacy statement
                Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView().CommandsRequested += (s, e) =>
                  e.Request.ApplicationCommands.Add(
                     new Windows.UI.ApplicationSettings.SettingsCommand("privacypolicy", Strings.PrivacyPolicy, async (o) =>
                     {
                         await Windows.System.Launcher.LaunchUriAsync(new Uri(Strings.PrivacyUrl));
                     })
                  );
            }
        }

        public async Task Start()
        {
            AddFeeds(false);

            // at first we will only reload from cache (so it's fast)
            RefreshFromCache();

            // setup in app purchase (simulate when debugging)
            if (Strings.IncludeAdvertising)
            {
                m_HideAdsFeature = await new InAppPurchaseHelper(HideAdsFeatureName, Strings.SimulatePurchasing).Setup();
                this.HideAds = m_HideAdsFeature.IsPurchased;
            }

            // share score
            Windows.ApplicationModel.DataTransfer.DataTransferManager
                .GetForCurrentView().DataRequested += (s, e) =>
                {
                    var _String = string.Format(Strings.ShareHtml,
                        SelectedArticle.Title, SelectedArticle.Author,
                        SelectedArticle.Date, SelectedArticle.Url,
                        this.Feeds.First(x => x.Articles.Contains(SelectedArticle)).Feed.MoreUrl);
                    var _Html = Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.CreateHtmlFormat(_String);
                    e.Request.Data.Properties.Title = Strings.ShareTitle;
                    e.Request.Data.Properties.Description = string.Empty;
                    e.Request.Data.SetUri(new Uri(SelectedArticle.Url));
                    e.Request.Data.SetHtmlFormat(_Html);
                    SelectedArticle = null;
                };
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Equals("SelectedArticle"))
                {
                    if (SelectedArticle is SummaryArticle) SelectedArticle = null;
                    if (SelectedArticle is AdvertArticle) SelectedArticle = null;
                    if (SelectedArticle is WeatherArticle) SelectedArticle = null;
                    //if (SelectedArticle != null)
                        //Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
                }
            };

            // update location
            if (Strings.IncludeAdvertising)
                try
                {
                    var _Location = new Windows.Devices.Geolocation.Geolocator();
                    var _Position = await _Location.GetGeopositionAsync();
                    this.Latitude = _Position.Coordinate.Latitude;
                    this.Longitude = _Position.Coordinate.Longitude;
                }
                catch
                {
                    // this breakpoint may be reached if the user BLOCKS location services
                    Debug.WriteLine("Error reading location; user may have BLOCKED");
                    System.Diagnostics.Debugger.Break();
                }
        }

        private void SetTileNotification(string text)
        {
            Windows.Data.Xml.Dom.XmlDocument _Tile = null;

            // a template without an image
            _Tile = Windows.UI.Notifications.TileUpdateManager
                .GetTemplateContent(Windows.UI.Notifications.TileTemplateType.TileSquareText04);
            var _Texts = _Tile.GetElementsByTagName("text");
            _Texts[0].InnerText = text;
            var _Notification = new Windows.UI.Notifications.TileNotification(_Tile) { Tag = "no-repeat" };
            var _Updater = Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication();
            _Updater.Clear();
            _Updater.Update(_Notification);
        }

        private static void SetTileBadge(int value)
        {
            // update live tile badge
            var _Badge = Windows.UI.Notifications.BadgeUpdateManager
                .GetTemplateContent(Windows.UI.Notifications.BadgeTemplateType.BadgeNumber);
            var _XmlElement = _Badge.SelectSingleNode("/badge") as Windows.Data.Xml.Dom.XmlElement;
            if (_XmlElement != null)
                _XmlElement.SetAttribute("value", value.ToString());
            var _Note = new Windows.UI.Notifications.BadgeNotification(_Badge);
            Windows.UI.Notifications.BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(_Note);
        }

        readonly ObservableCollection<GroupedItem> m_Feeds = new ObservableCollection<GroupedItem>();
        public ObservableCollection<GroupedItem> Feeds { get { return m_Feeds; } }

        double m_Longitude = default(double);
        public double Longitude { get { return m_Longitude; } set { SetProperty(ref m_Longitude, value); } }

        double m_Latitude = default(double);
        public double Latitude { get { return m_Latitude; } set { SetProperty(ref m_Latitude, value); } }

        public string Title { get { return Strings.Title; } }
        public string SubTitle { get { return Strings.SubTitle; } }

        ArticleBase m_SelectedArticle = default(ArticleBase);
        public ArticleBase SelectedArticle { get { return m_SelectedArticle; } set { SetProperty(ref m_SelectedArticle, value); } }

        #region Refresh

        async void RefreshFromCache(int minuteThreshold = 15)
        {
            var _Feeds = this.Feeds
                .Where(x => x.Feed != null)
                .Where(x => x.Feed.SourceUrl != null)
                .Where(x => !string.IsNullOrEmpty(x.Feed.SourceUrl));
            foreach (var _Item in _Feeds)
                await RefreshFromCache(_Item, minuteThreshold);
        }

        readonly List<IFeed> m_BusyRefreshingFromCache = new List<IFeed>();
        async Task RefreshFromCache(GroupedItem group, int minuteThreshold)
        {
            if (m_BusyRefreshingFromCache.Contains(group.Feed))
                return;
            m_BusyRefreshingFromCache.Add(group.Feed);
            if (await group.Feed.ReadFromCache())
                if (!group.Feed.Articles.Any())
                    await RefreshFromWeb(group);
                else if (Math.Abs(group.Feed.Updated.Subtract(DateTime.Now).TotalMinutes) < minuteThreshold)
                    Refresh(group);
                else
                    await RefreshFromWeb(group);
            else
                await RefreshFromWeb(group);
            m_BusyRefreshingFromCache.Remove(group.Feed);
        }

        async Task RefreshFromWeb()
        {
            if (!App.IsNetworkOkay)
                return;
            foreach (var _Group in this.Feeds.Where(x => !string.IsNullOrEmpty(x.Feed.SourceUrl)))
                await RefreshFromWeb(_Group);
        }

        readonly List<IFeed> m_BusyRefreshingFromWeb = new List<IFeed>();
        async Task RefreshFromWeb(GroupedItem group)
        {
            if (!App.IsNetworkOkay)
            {
                await new Windows.UI.Popups.MessageDialog(Strings.NoInternetWarning).ShowAsync();
                return;
            }
            if (m_BusyRefreshingFromWeb.Contains(group.Feed))
                return;
            m_BusyRefreshingFromWeb.Add(group.Feed);
            if (await group.Feed.ReadFromWeb())
                Refresh(group);
            else
            {
                Debug.WriteLine("Error reading feed {0} from web", group.Feed.SourceUrl);
                Debugger.Break();
            }
            m_BusyRefreshingFromWeb.Remove(group.Feed);
        }

        void Refresh(GroupedItem groupedItem)
        {
            Debug.Assert(groupedItem != null);
            Debug.Assert(groupedItem.Feed != null);
            Debug.Assert(groupedItem.Articles != null);

            var _Items = groupedItem.Feed.Articles.ToArray();
            foreach (var _Item in _Items)
                FormatItem(_Item);

            var _RemoveThese = groupedItem.Articles.ToArray()
                .Where(x => !_Items.Select(y => y.Url).Contains(x.Url));
            foreach (var _Item in _RemoveThese)
                groupedItem.Articles.Remove(_Item);

            var _UpdateThese =
                from a in groupedItem.Articles
                join i in _Items on a.Url equals i.Url
                select new { Old = a, New = i };
            foreach (var _Item in _UpdateThese)
                _Item.Old.MapProperties(_Item.New);

            var _InsertThese = _Items.ToArray()
                .Where(x => !groupedItem.Articles.Select(y => y.Url).Contains(x.Url));
            foreach (var _Item in _InsertThese.Select((x, i) => new { Item = x, Index = i }))
            {
                _Item.Item.Index = _Item.Index;
                FormatItem(_Item.Item);
                groupedItem.Articles.Insert(0, _Item.Item);
            }

            // update hero in the grid
            if (groupedItem.Articles.Any())
            {
                var _First = groupedItem.Articles.First();
                groupedItem.Articles.Remove(_First);
                _First.Index = 0;
                FormatItem(_First);
                groupedItem.Articles.Insert(0, _First);
            }

            // update and adverts
            FormatItems(groupedItem.Articles);
        }

        #endregion

        #region ReloadAll DelegateCommand

        DelegateCommand m_ReloadAllCommand = null;
        public DelegateCommand ReloadAllCommand
        {
            get
            {
                if (m_ReloadAllCommand != null)
                    return m_ReloadAllCommand;
                m_ReloadAllCommand = new DelegateCommand(
                    ReloadAllCommandExecute, ReloadAllCommandCanExecute);
                this.PropertyChanged += (s, e) => m_ReloadAllCommand.RaiseCanExecuteChanged();
                return m_ReloadAllCommand;
            }
        }
        async void ReloadAllCommandExecute() { await RefreshFromWeb(); }
        bool ReloadAllCommandCanExecute() { return true; }

        #endregion

        #region LoadFeed DelegateCommand

        DelegateCommand<string> m_LoadFeedCommand = null;
        public DelegateCommand<string> LoadFeedCommand
        {
            get
            {
                if (m_LoadFeedCommand != null)
                    return m_LoadFeedCommand;
                m_LoadFeedCommand = new DelegateCommand<string>(
                    LoadFeedCommandExecute, LoadFeedCommandCanExecute);
                this.PropertyChanged += (s, e) => m_LoadFeedCommand.RaiseCanExecuteChanged();
                return m_LoadFeedCommand;
            }
        }
        async void LoadFeedCommandExecute(string moreUrl) { await Windows.System.Launcher.LaunchUriAsync(new Uri(moreUrl)); }
        bool LoadFeedCommandCanExecute(string moreUrl) { return !string.IsNullOrEmpty(moreUrl); }

        #endregion

        #region PurchaseHideAds DelegateCommand

        bool m_HideAds = false;
        public bool HideAds
        {
            get
            {
                if (!Strings.IncludeAdvertising)
                    return true;
                return m_HideAds;
            }
            set { SetProperty(ref m_HideAds, value); }
        }

        const string HideAdsFeatureName = "HideAds";
        InAppPurchaseHelper m_HideAdsFeature;

        DelegateCommand m_PurchaseHideAdsCommand = null;
        public DelegateCommand PurchaseHideAdsCommand
        {
            get
            {
                if (m_PurchaseHideAdsCommand != null)
                    return m_PurchaseHideAdsCommand;
                m_PurchaseHideAdsCommand = new DelegateCommand(
                    PurchaseHideAdsCommandExecute, PurchaseHideAdsCommandCanExecute);
                this.PropertyChanged += (s, e) => m_PurchaseHideAdsCommand.RaiseCanExecuteChanged();
                return m_PurchaseHideAdsCommand;
            }
        }
        async void PurchaseHideAdsCommandExecute()
        {
            await m_HideAdsFeature.Purchase();
            HideAds = m_HideAdsFeature.IsPurchased;
        }
        bool PurchaseHideAdsCommandCanExecute()
        {
            if (!Strings.IncludeAdvertising)
                return false;
            if (m_HideAdsFeature == null)
                return false;
            if (!m_HideAdsFeature.CanPurchase)
                return false;
            if (m_HideAdsFeature.IsPurchased)
            {
                foreach (var _Item in Feeds)
                {
                    var _Ads = _Item.Articles.OfType<AdvertArticle>();
                    foreach (var _Ad in _Ads.ToArray())
                        _Item.Articles.Remove(_Ad);
                }
            }
            return !m_HideAdsFeature.IsPurchased;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        void SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (!object.Equals(storage, value))
            {
                storage = value;
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
