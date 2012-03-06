using Template.Config;
using Template.Config.Content;

namespace Template.SourceUrls
{
    /// <summary>
    /// Implement ISourceUrl to determine the download URL for
    /// a type of feed (see ActivityTypes in the Configuration.xaml)
    /// </summary>
    public interface ISourceUrl
    {
        /// <summary>
        /// Implement the SourceDownloadUrl method to return the URL that 
        /// should be downloaded to retrieve activities
        /// </summary>
        /// <param name="source">The social source being downloaded. Access
        /// the Id property to customise the download url. For example this
        /// could be the page id of a Facebook page. In the case of an Atom
        /// or RSS feed, the Id would contain the full download URL</param>
        /// <param name="more">False the first time the source is downloaded
        /// True on subsequent downloads. You can use this to determine 
        /// whether to do an incremental download. </param>
        /// <returns>Return the URL that should be downloaded to retrieve
        /// activities for the specified source</returns>
        string SourceDownloadUrl(SocialSource source, bool more);

        SocialProvider AssociatedNetwork { get; set; }
    }
}
