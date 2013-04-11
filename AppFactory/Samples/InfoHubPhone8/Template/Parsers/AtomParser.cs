using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BuiltToRoam;
using InfoHubPhone8.Config.Content;
using InfoHubPhone8.Data;

namespace InfoHubPhone8.Parsers
{
    /// <summary>
    /// Parses content from an Atom feed - http://www.w3.org/2005/Atom
    /// </summary>
    public class AtomParser : ISourceParser
    {
        public IEnumerable<SocialActivity> Parse(SocialSource source, Stream activityStream)
        {
            var xelement = XElement.Load(activityStream);
            var ns = XNamespace.Get("http://www.w3.org/2005/Atom");
            var activities = (from entry in xelement.Descendants(ns.GetName("entry"))
                              let title = entry.SafeElementValue(ns.GetName("title"))
                              let description = entry.SafeElementValue(ns.GetName("content"))
                              let pubDate = entry.SafeElementValue(ns.GetName("published"))
                              let updatedDate = entry.SafeElementValue(ns.GetName("updated"))
                              let summary =
                                  entry.SafeElementValue(ns.GetName("summary")).Replace("[...]", "")
                              select new SocialActivity
                                         {
                                             Id = entry.SafeElementValue(ns.GetName("id")),
                                             Url =
                                                 entry.Element(ns.GetName("link")).SafeAttributeValue(
                                                     "href"),
                                             Title = title,
                                             Description = description,
                                             TimeStamp = (!string.IsNullOrEmpty(pubDate) ? pubDate : updatedDate).ToDateWithTimezoneSupport() ?? DateTime.Now
                                         });
            return activities;
        }
    }
}