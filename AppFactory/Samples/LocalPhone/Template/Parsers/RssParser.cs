using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BuiltToRoam;
using Template.Config.Content;
using Template.Data;

namespace Template.Parsers
{
    /// <summary>
    /// Decodes RSS feed data
    /// </summary>
    public class RssParser : ISourceParser
    {
        public IEnumerable<SocialActivity> Parse(SocialSource source, Stream activityStream)
        {
            var ns = XNamespace.Get("http://purl.org/rss/1.0/modules/content/");
            var xelement = XElement.Load(activityStream);
            var activities = (from entry in xelement.Descendants("item")
                              let title = entry.SafeElementValue("title")
                              let content = entry.SafeElementValue(ns.GetName("encoded")) + ""
                              let description = !string.IsNullOrEmpty(content) ? content : entry.SafeElementValue("description") + ""
                              let summary = description.Substring(0, Math.Min(200, description.Length))
                              select new SocialActivity
                                         {
                                             Id = entry.SafeElementValue("guid"),
                                             Url = entry.SafeElementValue("link"),
                                             Title = title,
                                             Description = description,
                                             TimeStamp = entry.SafeElementValue("pubDate").ToDateWithTimezoneSupport() ?? DateTime.Now,
                                             NewEnclosures = ParseEnclosures(entry)
                                         });
            return activities;
        }

        /// <summary>
        /// Extracts information about enclosures and adds them to the activity
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static IEnumerable<Enclosure> ParseEnclosures(XElement entry)
        {
            var elements = (from enc in entry.Elements("enclosure")
                            let type = ParseEnclosureType(enc.SafeAttributeValue("type"))
                            let url = enc.SafeAttributeValue("url")
                            where !string.IsNullOrEmpty(url) && type != EnclosureOptions.None
                            select new Enclosure
                                       {
                                           Type = type,
                                           Url = url
                                       }).ToArray();

            return elements;
        }

        /// <summary>
        /// Determine the enclosure type based on the MIME type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static EnclosureOptions ParseEnclosureType(string type)
        {
            switch (type.ToLower())
            {
                case "image/jpeg":
                case "image/jpg":
                case "image/bmp":
                case "image/png":
                    return EnclosureOptions.Image;
                case "video/mpeg":
                case "video/mp4":
                case "video/wmv":
                case "video/x-ms-wmv":
                    return EnclosureOptions.Video;
            }
            return EnclosureOptions.None;
        }


    }
}
