using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using InfoHubPhone8.Config.Content;
using InfoHubPhone8.Data;
using InfoHubPhone8.SourceUrls;

namespace InfoHubPhone8.Parsers
{
    /// <summary>
    /// Decodes twitter feed data
    /// </summary>
    public class TwitterParser : ISourceParser
    {
        // Create a singleton serializer in case there are multiple twitter parser instances
        private static readonly DataContractJsonSerializer JsonSerializer = new DataContractJsonSerializer(typeof(TwitterFeed));

        public IEnumerable<SocialActivity> Parse(SocialSource source, Stream activityStream)
        {
            var twitterData = JsonSerializer.ReadObject(activityStream) as TwitterFeed;

            if (twitterData != null)
            {
                // Extract the twitter activities
                var activities = (from entry in twitterData.FeedItems
                                  select new SocialActivity
                                             {
                                                 Id = entry.Id,
                                                 Url = string.Format("http://twitter.com/{0}/statuses/{1}", entry.UserId, entry.Id),
                                                 Description = entry.Text,
                                                 Author = "@" + entry.UserId,
                                                 TimeStamp = entry.CreatedAt,
                                                 NewEnclosures = new[]
                                                                     {
                                                                         new Enclosure
                                                                             {
                                                                                 Type= EnclosureOptions.Image,
                                                                                 IconUrl = entry.ProfileImageUrl,
                                                                                 ImageUrl= entry.ProfileImageUrl
                                                                             }
                                                                     }
                                             });

                // Save the Next and Refresh Urls
                if (!string.IsNullOrWhiteSpace(twitterData.NextPage))
                {
                    var nextUrl = TwitterSourceUrl.TwitterSearchBaseUrl + twitterData.NextPage;
                    ReaderSettings.Instance.WriteMoreLink(source, nextUrl);
                }

                if (!string.IsNullOrWhiteSpace(twitterData.RefreshUrl))
                {
                    var refreshUrl = TwitterSourceUrl.TwitterSearchBaseUrl + twitterData.RefreshUrl;
                    ReaderSettings.Instance.WriteRefreshLink(source, refreshUrl);
                }


                return activities;
            }
            return null;
        }

        #region JSON Mapping Class
        [DataContract]
        public class TwitterFeed
        {
            [DataMember(Name = "next_page")]
            public string NextPage { get; set; }
            [DataMember(Name = "refresh_url")]
            public string RefreshUrl { get; set; }

            [DataMember(Name = "results")]
            public TwitterItem[] FeedItems { get; set; }

            [DataContract]
            public class TwitterItem
            {
                [DataMember(Name = "id_str")]
                public string Id { get; set; }
                [DataMember(Name = "created_at")]
                public DateTime CreatedAt { get; set; }
                [DataMember(Name = "from_user")]
                public string UserId { get; set; }
                [DataMember(Name = "profile_image_url")]
                public string ProfileImageUrl { get; set; }
                [DataMember(Name = "text")]
                public string Text { get; set; }
            }
        }
        #endregion
    }
}