using InfoHub.Articles;
using InfoHub.Feeds;
using InfoHub.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoHub
{
    public partial class AppHubViewModel
    {
        public static class Strings
        {
            // MAIN INFORMATION

            // this is the title and subtitle of the main page (above the grid)
            public const string Title = "$(Application.Title)";
            public const string SubTitle = "$(Application.SubTitle)";

            // Critical
            public const string SearchTerm1 = "$(Application.SearchTerm1)";
            //public const string SearchTerm2 = "Application.SearchTerm2";
            public const string SearchTerm3 = "Term3IfNeeded";
            public const string YouTubeAuthor = "$(Application.YouTubeAuthor)";             //YouTubeAuthor

            // Optional
            public const string TwitterUser = "$(Application.TwitterUser)";   //Only if using TwitterUser instead of search
            public const string FlickrUserName = "arthill";                 //Only if useing a flickr user instead of search
            public const string FlickrUserNum = "79847590@N00";             //Only if using a flickr user insead of search
            public const double WeatherLatitude = 39.75410d;                //Only if using Weather
            public const double WeatherLongitude = -105d;                   //Only if using Weather

            // SUMMARY / OVERVIEW SECTION

            // this "summary" is extra data to make your app more valuable
            //public const bool SummaryInclude = true;
            //public const string SummaryTitle = "Overview";
            //public const string SummaryMoreUrl = "http://en.wikipedia.org/wiki/Denver_Broncos";
            //public static object SummaryData = new
            //{
            //    Title1 = "SummaryLine1", // First line
            //    Title2 = "SummaryLine2", // Second line
            //    Image = "ms-appx:///Assets/SummaryImage.png", // Optional if you don't have one
            //    Facts = new string[][]
            //    { 
            //        // usually room for about 7 items (fewer is okay, too  comment out lines)
            //       new string[]{ "Fact1Key", "Fact1Value "},
            //    },
            //};

            // STANDARD NEWS FEED

            // any standard rss feed can be used here
            public const bool NewsInclude = true;
            public const string NewsTitle = "Current News";
            public const int NewsCount = 7;
            public const string NewsSourceUrl = "$(Application.RssNewsUrl)";
            public const string NewsMoreUrl = "$(Application.RssNewsMoreUrl)";

            // YOUTUBE

            // get your YouTube link here: https://gdata.youtube.com/demo/index.html
            public const bool YouTubeInclude = true;
            public const string YouTubeTitle = "Current Videos";
            public const int YouTubeCount = 5;

            public const string YouTubeSourceUrl = "http://gdata.youtube.com/feeds/base/videos?max-results=12&alt=rss&author=" + YouTubeAuthor;
            public const string YouTubeMoreUrl = "http://youtube.com/" + YouTubeAuthor;

            // PRIVACY POLICY

            // build a statement at: http://privacychoice.org
            // here's a generic policy: http://freeprivacypolicy.org/generic.php
            public const string PrivacyUrl = "http://freeprivacypolicy.org/generic.php";
            public const string PrivacyPolicy = "Privacy Policy";

            // FLICKR FEED

            // get your flickr link here: http://www.flickr.com/services/feeds/docs/photos_public/
            // example: single user http://api.flickr.com/services/feeds/photos_public.gne?id=36140829@N03&lang=en-us&format=rss_200
            // for a single user, look at the bottom of their photostream page for the rss button
            public const bool FlickrInclude = true;
            public const string FlickrTitle = "Recent Photos";
            public const int FlickrCount = 5;

            // Flickr (2 search terms)
            public const string FlickrSourceUrl = "http://api.flickr.com/services/feeds/photos_public.gne?tags=" + SearchTerm1 + "&tagmode=all&format=rss2"; // + "," + SearchTerm2
            public const string FlickrMoreUrl = "http://www.flickr.com/search/?q=" + SearchTerm1; //+ "%20" + SearchTerm2;

            // Flickr (3 search terms)
            // public const string FlickrSourceUrl = "http://api.flickr.com/services/feeds/photos_public.gne?tags=" + SearchTerm1 + "," + SearchTerm2 + "," + SearchTerm3 + "&tagmode=all&format=rss2";
            // public const string FlickrMoreUrl = "http://www.flickr.com/search/?q=" + SearchTerm1 + "%20" + SearchTerm2 + "%20" + SearchTerm3;

            // Flickr (user's feed)            
            // public const string FlickrSourceUrl = "http://api.flickr.com/services/feeds/photos_public.gne?id=" + SearchTerm1 + "," + SearchTerm2 + "&lang=en-us&format=rss_200";
            // public const string FlickrMoreUrl = "http://www.flickr.com/photos/" + FlickrUserName + "/";

            // TWITTER FEED

            // get your twitter link here: https://dev.twitter.com/docs/api/1/get/statuses/user_timeline
            // example: single user http://api.twitter.com/1/statuses/user_timeline.rss?screen_name=jerrynixon
            public const bool TwitterInclude = true;
            public const string TwitterTitle = "Latest Tweets";
            public const int TwitterCount = 12;

            // 2 Search terms
            //public const string TwitterSourceUrl = "http://search.twitter.com/search.rss?q=" + SearchTerm1 + "%20" + SearchTerm2 + "&rpp=20";
            //public const string TwitterMoreUrl = "https://twitter.com/search?q=" + SearchTerm1 + "%20" + SearchTerm2 + "&src=typd";

            // 3 search terms 
            // public const string TwitterSourceUrl = "http://search.twitter.com/search.rss?q=" + SearchTerm1 + "%20" + SearchTerm2 + "%20" + SearchTerm3 + "&rpp=20";
            // public const string TwitterMoreUrl = "https://twitter.com/search?q=" + SearchTerm1 + "%20" + SearchTerm2 + "%20" + SearchTerm3 + "&src=typd";

            // if using a twitter user instead of a search
            public const string TwitterSourceUrl = "http://api.twitter.com/1/statuses/user_timeline.rss?screen_name=" + TwitterUser + "&rpp=20";
            public const string TwitterMoreUrl = "https://twitter.com/search?q=" + SearchTerm1 + "%20" + "&src=typd"; //SearchTerm2 + 

            // LOCAL WEATHER

            // get your weather link here: http://forecast.weather.gov/
            public const bool WeatherInclude = false;
            public const string WeatherTitle = "Weather";
            public const int WeatherCount = 7; 


            // CALENDAR 
            // standard ics calendar can be used here
            // set to TRUE if you find one
            public const bool CalendarInclude = false;
            public const string CalendarTitle = "Schedule";
            public const int CalendarCount = 12;
            public const string CalendarSourceUrl = "http://www.southendzone.com/ical/broncos.ics";
            public const string CalendarMoreUrl = "http://www.denverbroncos.com/schedule-and-events/schedule.html";

            // PRIVACY POLICY & EMAIL 

            // Local Privacy Policy if Web Base is not enough - Add your email and set to true
            // can help if policy is issue passing
            public const bool PrivacyPolicyUseLocal = true;
            public const string PrivacyPolicyText = "Privacy Policy";
            public const string ContactEmail = "$(Application.ContactEmail)";
            public const string PrivacyPolicyLocalText = "This privacy policy governs your use of this application. "
                + "The  Application does not collect personal information and does not monitor your personal use of the Application. "
                + "This application does not transmit any information without your knowledge. "
                + "You can easily uninstall the application at any time by using the standard uninstall processes available with Windows platform. "
                + "If you have a reason to believe that your personal information is being tracked or collected while using the Application, please contact us at " + ContactEmail;

            // 0 == title of article
            // 1 == author of article
            // 2 == date of article
            // 3 == url of article
            // 4 == url of feed
            public const string ShareHtml = "Hey! <p>I wanted to share {0} by {1} on {2:d}. <p>It's from {3} in {4}. <p>Check it out!";
            public const string ShareTitle = "$(Application.Title) Information";

            // set to false to hide ads everywhere hardcoded
            public static bool IncludeAdvertising = $(Application.AdEnabled); // in general this should be left to true
            public static bool SimulatePurchasing = System.Diagnostics.Debugger.IsAttached;

            // get your ad values from http://pubcenter.microsoft.com
            public const string AdApplicationId = "43da88f7-2b36-46f3-81dd-0b043193e1c6";
            public const string AdUnitId = "10056298";

            public const string NoInternetWarning = "Internet is required. Check connection and refresh.";
        }
    }
}
