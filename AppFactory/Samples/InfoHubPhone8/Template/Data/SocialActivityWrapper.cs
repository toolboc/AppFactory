using System.Linq;
using System.Windows;
using BuiltToRoam;
using BuiltToRoam.BaseClasses;
using InfoHubPhone8.Config.Content;
using InfoHubPhone8.Config.Content.Lists;

namespace InfoHubPhone8.Data
{
    public class SocialActivityWrapper : NotifyBase, ISocialActivityWrapper
    {
        public SocialActivity SocialActivity { get; set; }

        public bool FullFunctionality { get; set; }

        public bool LimitedFunctionality { get; set; }

        public bool TrialModeOver { get; set; }

        public string SourceImage { get; set; }

        public SocialSource FeedSource { get; set; }

        public Enclosure FirstImage { get { return ImagesExist ? ImageEnclosures[0] : null; } }


        public Visibility DescriptionVisibility
        {
            get
            {
                return
                    (string.IsNullOrEmpty(SocialActivity.Description) && string.IsNullOrEmpty(SocialActivity.Title) && SocialActivity.Title != SocialActivity.Description).
                        ToVisibility();
            }

        }

        public string FriendlyTimeStamp
        {
            get { return SocialActivity.TimeStamp.FriendlyDateFormat(); }
        }

        public string ActivitySourceAndTime
        {
            get { return FeedSource.SourceName + ", " + FriendlyTimeStamp; }
        }

        public Visibility TitleVisibility
        {
            get { return (!string.IsNullOrEmpty(SocialActivity.Title)).ToVisibility(); }
        }

        public bool Read
        {
            get { return SocialActivity.Read; }
            set
            {
                if (Read == value) return;
                SocialActivity.Read = value;
                this.RaisePropertyChanged(() => Read);
                this.RaisePropertyChanged(() => NotRead);
                this.RaisePropertyChanged(() => ReadVisibility);
                this.RaisePropertyChanged(() => NotReadVisibility);
            }
        }

        public bool NotRead
        {
            get { return !Read; }
        }

        public Visibility ReadVisibility
        {
            get { return Read.ToVisibility(); }
        }

        public Visibility NotReadVisibility
        {
            get { return (!Read).ToVisibility(); }
        }

        private object enclosurelock = new object();
        private Enclosure[] imageEnclosures;
        public Enclosure[] ImageEnclosures
        {
            get
            {
                if (SocialActivity.Enclosures == null) return new Enclosure[] { };
                if (imageEnclosures == null)
                {
                    lock (enclosurelock)
                    {
                        imageEnclosures =
                            (SocialActivity.Enclosures.Where(enc => !string.IsNullOrWhiteSpace(enc.ImageUrl))).ToArray();
                    }
                }
                return imageEnclosures;
            }
        }

        private Enclosure[] videoEnclosures;
        public Enclosure[] VideoEnclosures
        {
            get
            {
                if (SocialActivity.Enclosures == null) return new Enclosure[] { };
                if (videoEnclosures == null)
                {
                    lock (enclosurelock)
                    {
                        videoEnclosures =
                            (SocialActivity.Enclosures.Where(enc => enc.Type == EnclosureOptions.Video)).ToArray();
                    }
                }
                return videoEnclosures;
            }

        }

        public bool ImagesExist
        {
            get
            {
                return ImageEnclosures.FirstOrDefault() != null;
            }
        }

        public Visibility ImageEnclosuresVisibility
        {
            get
            {
                return ImagesExist.ToVisibility();
            }
        }

        public bool VideosExist
        {
            get
            {
                return VideoEnclosures.FirstOrDefault() != null;
            }
        }

        public Visibility VideoEnclosuresVisibility
        {
            get
            {
                return VideosExist.ToVisibility();
            }
        }


        public bool HasShortenedUrl
        {
            get { return SocialActivity.ShortenedUrl != SocialActivity.Url; }
        }

    }
}