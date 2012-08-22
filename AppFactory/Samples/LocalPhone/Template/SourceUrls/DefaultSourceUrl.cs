using Template.Config;
using Template.Config.Content;

namespace Template.SourceUrls
{
    /// <summary>
    /// This is a default implementation of ISourceUrl
    /// which simply returns the source Id property, making
    /// the assumption that this includes the full download URL
    /// for the feed (eg for Atom/RSS feeds). Note that it 
    /// returns null when the more attribute is set to true
    /// </summary>
    public class DefaultSourceUrl : ISourceUrl
    {
        public SocialProvider AssociatedNetwork { get; set; }

        public virtual string SourceDownloadUrl(SocialSource source, bool more)
        {
            if (more) return null;
            return source.Id;
        }
    }
}
