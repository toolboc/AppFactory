using System.Collections.Generic;
using System.IO;
using Template.Config.Content;
using Template.Data;

namespace Template.Parsers
{
    /// <summary>
    /// Implement ISourceParse to define how a data feed is parsed. 
    /// </summary>
    public interface ISourceParser
    {
        /// <summary>
        /// The parse method is where all the magic happens. Once the 
        /// data feed has been downloaded, this method is called with 
        /// the SocialSource (ie the configuration info for the feed 
        /// source) and the stream of data to be parsed. Simply return
        /// an IEnumerable of activities.
        /// </summary>
        /// <param name="source">The configuration information about the feed
        /// source</param>
        /// <param name="activityStream">The stream of data retrieved
        /// from the feed</param>
        /// <returns>The collection of activities parsed from the feed</returns>
        IEnumerable<SocialActivity> Parse(SocialSource source, Stream activityStream);
    }

}