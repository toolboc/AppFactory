using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Template.Config.Content;
using Template.Data;

namespace Template.Parsers
{
    /// <summary>
    /// Decodes Flickr JSON feed data
    /// </summary>
    public class FlickrParser : ISourceParser
    {
        // Create a singleton serializer in case there are multiple Flickr parser instances
        private static readonly DataContractJsonSerializer JsonSerializer = new DataContractJsonSerializer(typeof(FlickrWrapper));

        public IEnumerable<SocialActivity> Parse(SocialSource source, Stream activityStream)
        {
            // Read off the flickr prefix
            using (var reader = new StreamReader(activityStream))
            {
                var sb = new StringBuilder(reader.ReadToEnd());
                const string prefix = "jsonFlickrFeed(";
                sb.Remove(0, prefix.Length);
                sb.Remove(sb.Length - 1, 1);
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(sb.ToString())))
                {
                    var data = JsonSerializer.ReadObject(ms) as FlickrWrapper;


                    if (data == null || data.Items == null) return new SocialActivity[] { };
                    return (from msg in data.Items
                            select new SocialActivity
                                       {
                                           Id = msg.Link,
                                           Title = msg.Title,
                                           Description = msg.Description,
                                           Url = msg.Link,
                                           TimeStamp = msg.DateTaken,
                                           NewEnclosures = new[]
                                                               {
                                                                   new Enclosure
                                                                       {
                                                                           Caption = msg.Title,
                                                                           ImageUrl = msg.Media.ImageUrl,
                                                                           Url = msg.Media.ImageUrl,
                                                                           IconUrl = msg.Media.ImageUrl,
                                                                           Type = EnclosureOptions.Image
                                                                       }
                                                               }
                                       });
                }
            }
        }

        #region JSON Mapping Class

        [DataContract]
        public class FlickrWrapper
        {
            [DataMember(Name = "items")]
            public FlickrItem[] Items { get; set; }

            [DataContract]
            public class FlickrItem
            {
                [DataMember(Name = "title")]
                public string Title { get; set; }

                [DataMember(Name = "link")]
                public string Link { get; set; }

                [DataMember(Name = "description")]
                public string Description { get; set; }

                [DataMember(Name = "date_taken")]
                public DateTime DateTaken { get; set; }

                [DataMember(Name = "media")]
                public MediaData Media { get; set; }

                [DataMember(Name = "author")]
                public string Author { get; set; }

                [DataContract]
                public class MediaData
                {
                    [DataMember(Name = "m")]
                    public string ImageUrl { get; set; }
                }
            }
        }

        #endregion
    }

}