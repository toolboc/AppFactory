using System.Net;
using Template.Config;
using Template.Config.Content;

namespace Template.SourceUrls
{
    /// <summary>
    /// Determines the download Url for Flickr feed. Doesn't
    /// current support downloading more items. Source.Id property
    /// is the search tag for Flickr images
    /// </summary>
    public class FlickrSourceUrl : DefaultSourceUrl
    {
        public override string SourceDownloadUrl(SocialSource source, bool more)
        {
            // ignore requests to download more items
            if (more) return null;

            string search = string.Empty;
            if (!string.IsNullOrWhiteSpace(source.Id))
            {
                // Combine the static Url with the source Id property to customize the search
                search = string.Format("&tags={0}", HttpUtility.UrlEncode(source.Id));
            }

            return "http://api.flickr.com/services/feeds/photos_public.gne?format=json" + search;
        }
    }
}