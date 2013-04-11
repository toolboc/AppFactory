using System;
using BuiltToRoam.Settings;
using InfoHubPhone8.Config;
using InfoHubPhone8.Config.Content;

namespace InfoHubPhone8.Data
{
    public class ReaderSettings : ApplicationSettings
    {
        private const string MorePrefix = "__more_prefix";
        private const string RefreshPrefix = "__refresh_prefix";

        /// <summary>
        /// Singleton of the ReaderSettings object
        /// </summary>
        public static ReaderSettings Instance { get; private set; }

        static ReaderSettings()
        {
            Instance = new ReaderSettings();
        }

        public bool UpdateCompletedOnLaunch
        {
            get { return RetrieveSetting(() => UpdateCompletedOnLaunch); }
            set
            {
                WriteSetting(() => UpdateCompletedOnLaunch, value);
            }
        }

        public DateTime FirstRun
        {
            get
            {
                if (!ValueExists(() => FirstRun))
                {
                    var firstRun = DateTime.Now;
                    WriteSetting(() => FirstRun, firstRun);
                }
                return RetrieveSetting(() => FirstRun);
            }
        }

        private string SourceToMoreKey(SocialSource source)
        {
            return MorePrefix + source.SourceName;
        }

        public void WriteMoreLink(SocialSource source, string moreLink)
        {
            WriteSetting(SourceToMoreKey(source), moreLink);
        }

        public string RetrieveMoreLink(SocialSource source)
        {
            return RetrieveSetting<string>(SourceToMoreKey(source));
        }

        private string SourceToRefreshKey(SocialSource source)
        {
            return RefreshPrefix + source.SourceName;
        }

        public void WriteRefreshLink(SocialSource source, string refreshLink)
        {
            WriteSetting(SourceToRefreshKey(source), refreshLink);
        }

        public string RetrieveRefreshLink(SocialSource source)
        {
            return RetrieveSetting<string>(SourceToRefreshKey(source));
        }

    }
}
