using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InfoHub.Articles
{
    [DataContract]
    public class ArticleBase : INotifyPropertyChanged, IArticle
    {
        public ArticleBase() { /* not used */ }
        public ArticleBase(Windows.Web.Syndication.SyndicationItem x)
        {
            // TODO: Complete member initialization
        }

        public void MapProperties(IArticle article)
        {
            this.Title = article.Title;
            this.Body = article.Body;
            this.Image = article.Image;
            this.Url = article.Url;
            this.Date = article.Date;
            this.Author = article.Author;
            this.Index = article.Index;
            this.ColSpan = article.ColSpan;
            this.RowSpan = article.RowSpan;
        }

        public virtual void MapProperties(SyndicationItem item, SyndicationFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(item != null, "item != null");

            if (item.Title != null)
                this.Title = item.Title.Text;
            switch (feed.SourceFormat)
            {
                case SyndicationFormat.Atom10:
                    this.Body = item.Content.Text;
                    break;
                case SyndicationFormat.Rss20:
                    this.Body = item.Summary.Text;
                    break;
            }
            this.Image = string.Empty;
            this.Date = item.PublishedDate.LocalDateTime;
            try { this.Url = item.Id; }
            catch
            {
                try { this.Url = item.Links.First().Uri.ToString(); }
                catch { }
            }
            try { this.Author = item.Authors.First().Name; }
            catch { }
        }

        #region properties

        string m_Title = default(string);
        [DataMember]
        public string Title { get { return m_Title; } set { SetProperty(ref m_Title, value); } }

        string m_Body = default(string);
        [DataMember]
        public string Body { get { return m_Body; } set { SetProperty(ref m_Body, value); } }

        string m_Image = default(string);
        [DataMember]
        public string Image { get { return m_Image; } set { SetProperty(ref m_Image, value); } }

        string m_Url = default(string);
        [DataMember]
        public string Url { get { return m_Url; } set { SetProperty(ref m_Url, value); } }

        DateTime m_Date = default(DateTime);
        [DataMember]
        public DateTime Date { get { return m_Date; } set { SetProperty(ref m_Date, value); } }

        string m_Author = default(string);
        [DataMember]
        public string Author { get { return m_Author; } set { SetProperty(ref m_Author, value); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (!object.Equals(storage, value))
            {
                storage = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        // layout oriented properties
        public int Index { get; set; }
        public bool Hero { get { return Index == 0; } }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
    }
}
