using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Template.Config.Content;
using Template.Data;

namespace Template.Parsers
{
    /// <summary>
    /// Parses content from Facebook
    /// </summary>
    public class FacebookParser : ISourceParser
    {
        // Create a singleton serializer in case there are multiple Facebook parser instances
        private static readonly DataContractJsonSerializer JsonSerializer =
            new DataContractJsonSerializer(typeof(DataWrapper));

        public IEnumerable<SocialActivity> Parse(SocialSource source, Stream activityStream)
        {
            var data = JsonSerializer.ReadObject(activityStream) as DataWrapper;

            if (data == null || data.Data == null || data.Data.Length == 0) return new SocialActivity[] { };

            // Write out the links for Next and Refresh (ie previous in this case)
            ReaderSettings.Instance.WriteMoreLink(source, data.Paging.Next);
            ReaderSettings.Instance.WriteRefreshLink(source, data.Paging.Previous);

            return (from msg in data.Data
                    select new SocialActivity
                               {
                                   Author = (msg.From != null) ? msg.From.Name : null,
                                   Id = msg.Id,
                                   Title = msg.Title,
                                   Description = msg.Message,
                                   Url = msg.Url,
                                   TimeStamp = msg.Created,
                                   NewEnclosures = BuildLinks(msg)
                               });


        }

        /// <summary>
        /// Add the contents of the Facebook post as an enclosure to expose
        /// Image and Video content within the application
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static IEnumerable<Enclosure> BuildLinks(DataWrapper.MessageData msg)
        {
            var list = new List<Enclosure>();
            var img = ExtractImageUrl(msg.Picture);
            img = !string.IsNullOrWhiteSpace(img) ? img : msg.Icon;
            var url = !string.IsNullOrWhiteSpace(msg.Link) ? msg.Link : msg.Url;
            switch (msg.Type)
            {
                case "video":
                    list.Add(new Enclosure
                                 {
                                     Url = url,
                                     IconUrl = img,
                                     Caption = msg.Description,
                                     ImageUrl = img,
                                     Type = EnclosureOptions.Video
                                 });
                    break;
                case "link":
                    list.Add(new Enclosure
                                 {
                                     Url = url,
                                     IconUrl = img,
                                     Caption = msg.Description,
                                     ImageUrl = img,
                                     Type = EnclosureOptions.Link
                                 });
                    break;
                case "photo":
                    list.Add(new Enclosure
                                 {
                                     Url = url,
                                     IconUrl = img,
                                     Caption = msg.Description,
                                     ImageUrl = img,
                                     Type = EnclosureOptions.Image
                                 });
                    break;
                case "status":
                    list.Add(new Enclosure
                                 {
                                     Url = url,
                                     IconUrl = img,
                                     Caption = msg.Message,
                                     Type = EnclosureOptions.Link
                                 });
                    break;
            }
            return list.ToArray();
        }

        /// <summary>
        /// Extract the image url
        /// </summary>
        /// <param name="pictureUrl"></param>
        /// <returns></returns>
        private static string ExtractImageUrl(string pictureUrl)
        {
            if (string.IsNullOrEmpty(pictureUrl)) return null;
            var bits = pictureUrl.Split(new[] { '=', '&' });
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == "url")
                {
                    var url = bits[i + 1];
                    url = System.Net.HttpUtility.UrlDecode(url);
                    return url;
                }
            }
            return pictureUrl;
        }
    }

    #region JSON Mapping Classes
    [DataContract]
    public class DataWrapper
    {
        [DataMember(Name = "data")]
        public MessageData[] Data { get; set; }

        [DataMember(Name = "paging")]
        public MessageData.PagingData Paging { get; set; }


        [DataContract]
        public class MessageData
        {
            [DataMember(Name = "from")]
            public FacebookPoster From { get; set; }

            [DataContract]
            public class FacebookPoster
            {
                [DataMember(Name = "category")]
                public string Category { get; set; }

                [DataMember(Name = "name")]
                public string Name { get; set; }
            }

            [DataMember(Name = "id")]
            public string Id { get; set; }

            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "message")]
            public string Message { get; set; }

            [DataMember(Name = "type")]
            public string Type { get; set; }

            [DataMember(Name = "icon")]
            public string Icon { get; set; }


            [DataMember(Name = "link")]
            public string Link { get; set; }

            [DataMember(Name = "caption")]
            public string Caption { get; set; }

            [DataMember(Name = "picture")]
            public string Picture { get; set; }


            [DataMember(Name = "description")]
            public string Description { get; set; }

            [DataMember(Name = "created_time")]
            public DateTime Created { get; set; }

            [IgnoreDataMember]
            public string Title
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(Message)) return Name;
                    switch (Type)
                    {
                        case "status":
                        case "photo":
                            return Message.Length < 50 ? Message : Message.Substring(0, 50);
                        default:
                            return Name;
                    }
                }
            }

            private string url;

            [IgnoreDataMember]
            public string Url
            {
                get
                {
                    if (url == null)
                    {
                        var bits = Id.Split('_');
                        if (bits.Length >= 2)
                        {
                            url = string.Format("http://www.facebook.com/{0}/posts/{1}", bits[0], bits[1]);
                        }
                        else if (bits.Length == 1)
                        {
                            url = string.Format("http://www.facebook.com/{0}", bits[0]);
                        }
                        else
                        {
                            url = "http://www.facebook.com";
                        }
                    }
                    return url;
                }
            }

            [DataMember(Name = "likes")]
            public LikesData Likes { get; set; }

            [DataContract]
            public class LikesData
            {
                [DataMember(Name = "count")]
                public int Count { get; set; }
            }

            [DataContract]
            public class PagingData
            {
                [DataMember(Name = "previous")]
                public string Previous { get; set; }

                [DataMember(Name = "next")]
                public string Next { get; set; }

            }
        }
    #endregion
    }

}