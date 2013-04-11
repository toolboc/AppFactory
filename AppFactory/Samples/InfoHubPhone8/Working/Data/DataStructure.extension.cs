using System.Collections.Generic;

namespace InfoHubPhone8.Data
{
    public enum EnclosureOptions
    {
        None,
        Image,
        Video,
        Link
    }

    public partial class SocialActivity
    {
        public IEnumerable<Enclosure> NewEnclosures
        {
            set
            {
                if (value == null) return;
                Enclosures.AddRange(value);
            }
        }

        partial void OnUrlChanged()
        {
            if (!string.IsNullOrWhiteSpace(Url))
            {
                ShortenedUrl = Url;
            }
        }
    }


    public partial class Enclosure
    {
        public EnclosureOptions Type
        {
            get { return (EnclosureOptions)EnclosureType; }
            set { EnclosureType = (short)value; }
        }
    }

    //[DataContract]
    //public class Enclosure
    //{
    //    [DataMember]
    //    public EnclosureOptions Type { get; set; }
    //    [DataMember]
    //    public string Url { get; set; }
    //    [DataMember]
    //    public string Caption { get; set; }
    //    [DataMember]
    //    public string IconUrl { get; set; }
    //    [DataMember]
    //    public string ImageUrl { get; set; }
    //}

    //[DataContract]
    //public class SocialActivity : NotifyBase
    //{
    //    [DataMember]
    //    public string Id { get; set; }

    //    [DataMember]
    //    public DateTime TimeStamp { get; set; }

    //    [DataMember]
    //    public string Description { get; set; }

    //    [DataMember]
    //    public string Author { get; set; }

    //    [DataMember]
    //    public string Title { get; set; }

    //    [DataMember]
    //    public string Url { get; set; }

    //    [DataMember]
    //    public string FeedSourceName { get; set; }

    //    [DataMember]
    //    public Enclosure[] Enclosures { get; set; }

    //    [DataMember]
    //    public bool Read { get; set; }


    //    private string shortenedUrl;

    //    [DataMember]
    //    public string ShortenedUrl
    //    {
    //        get
    //        {
    //            if (!string.IsNullOrEmpty(shortenedUrl))
    //            {
    //                return shortenedUrl;
    //            }
    //            return Url;
    //        }
    //        set { shortenedUrl = value; }
    //    }

    //    [DataMember]
    //    public int Likes { get; set; }
    //}
}
