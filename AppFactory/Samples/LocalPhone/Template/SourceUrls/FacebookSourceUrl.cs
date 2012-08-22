using Template.Config;
using Template.Config.Content;
using Template.Data;

namespace Template.SourceUrls
{
    /// <summary>
    /// Determines the download Url for Facebook pages
    /// </summary>
    public class FacebookSourceUrl : DefaultSourceUrl
    {
        public override string SourceDownloadUrl(SocialSource source, bool more)
        {
            // In order to download from Facebook pages you need to supply an api key and api secret
            if (AssociatedNetwork == null
                || string.IsNullOrWhiteSpace(AssociatedNetwork.ApiKey)
                || string.IsNullOrWhiteSpace(AssociatedNetwork.ApiSecret))
            {
                return null;
            }

            // Determine what the refresh/more link is (based on previous downloads)
            string url = more ? ReaderSettings.Instance.RetrieveMoreLink(source) : ReaderSettings.Instance.RetrieveRefreshLink(source);
            if (!string.IsNullOrEmpty(url)) return url;

            // Build up the download URL based on the page Id (ie the source.Id property)
            return string.Format("https://graph.facebook.com/{0}/posts?access_token={1}|{2}", source.Id, AssociatedNetwork.ApiKey, AssociatedNetwork.ApiSecret);
        }
    }
}