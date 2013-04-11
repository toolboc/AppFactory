using System.Net;
using InfoHubPhone8.Config;
using InfoHubPhone8.Config.Content;
using InfoHubPhone8.Data;

namespace InfoHubPhone8.SourceUrls
{
    /// <summary>
    /// Determine the Twitter download Url. Source.Id 
    /// property is the search criteria
    /// </summary>
    public class TwitterSourceUrl : DefaultSourceUrl
    {
        public const string TwitterSearchBaseUrl = "http://search.twitter.com/search.json";

        public override string SourceDownloadUrl(SocialSource source, bool more)
        {
            string url = more ? ReaderSettings.Instance.RetrieveMoreLink(source) : ReaderSettings.Instance.RetrieveRefreshLink(source);
            if (!string.IsNullOrEmpty(url)) return url;

            return string.Format(TwitterSearchBaseUrl + "?q={0}", HttpUtility.UrlEncode(source.Id));
        }
    }
}