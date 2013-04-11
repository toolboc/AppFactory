using InfoHub.Articles;
using InfoHub.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InfoHub.Feeds
{
    [DataContract]
    [KnownType(typeof(AdvertArticle))]
    [KnownType(typeof(ArticleBase))]
    [KnownType(typeof(CalendarArticle))]
    [KnownType(typeof(FlickrArticle))]
    [KnownType(typeof(NewsArticle))]
    [KnownType(typeof(SummaryArticle))]
    [KnownType(typeof(TwitterArticle))]
    [KnownType(typeof(WeatherArticle))]
    [KnownType(typeof(YouTubeArticle))]
    public class FeedBase : INotifyPropertyChanged, IFeed
    {
        public virtual Task<bool> ReadFromWeb() { throw new NotImplementedException(); }

        public async Task<bool> ReadFromCache()
        {
            try
            {
                var _Cache = await StorageHelper.ReadFileAsync<FeedBase>(this.StorageKey, StorageHelper.StorageStrategies.Local);
                this.MapProperties(_Cache);
                return true;
            }
            catch { return false; }
        }

        protected async Task<bool> WriteToCache()
        {
            try
            {
                await StorageHelper.WriteFileAsync(this.StorageKey, this, StorageHelper.StorageStrategies.Local);
                var _Cache = await StorageHelper.ReadFileAsync<FeedBase>(this.StorageKey, StorageHelper.StorageStrategies.Local);
                return (_Cache != null) && (_Cache.Title == this.Title);
            }
            catch (Exception)
            {
                System.Diagnostics.Debugger.Break();
                return false;
            }
        }

        protected void MapProperties(FeedBase feed)
        {
            this.Title = feed.Title;
            this.SourceUrl = feed.SourceUrl;
            this.MoreUrl = feed.MoreUrl;
            this.Updated = feed.Updated;
            this.Image = feed.Image;
            this.Articles.Clear();
            MergeArticles(feed.Articles);
        }

        protected virtual void MergeArticles(IEnumerable<IArticle> items)
        {
            this.Articles.RemoveAll(x => x != null);
            this.Articles.AddRange(items.OrderBy(x => x.Date));
        }

        protected string StorageKey { get { return "FeedCache-" + this.SourceUrl.GetHashCode().ToString(); } }

        #region properties

        bool m_Busy = default(bool);
        public bool Busy { get { return m_Busy; } set { SetProperty(ref m_Busy, value); } }

        string m_Title = default(string);
        [DataMember]
        public string Title { get { return m_Title; } set { SetProperty(ref m_Title, value); } }

        string m_Image = default(string);
        [DataMember]
        public string Image { get { return m_Image; } set { SetProperty(ref m_Image, value); } }

        string m_SourceUrl = default(string);
        [DataMember]
        public string SourceUrl { get { return m_SourceUrl; } set { SetProperty(ref m_SourceUrl, value); } }

        string m_MoreUrl = default(string);
        [DataMember]
        public string MoreUrl { get { return m_MoreUrl; } set { SetProperty(ref m_MoreUrl, value); } }

        DateTime m_Updated = default(DateTime);
        [DataMember]
        public DateTime Updated { get { return m_Updated; } set { SetProperty(ref m_Updated, value); } }

        private List<IArticle> m_Articles = new List<IArticle>();
        [DataMember]
        public List<IArticle> Articles { get { return m_Articles; } set { m_Articles = value; } }

        public event PropertyChangedEventHandler PropertyChanged;
        void SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (!object.Equals(storage, value))
            {
                storage = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
