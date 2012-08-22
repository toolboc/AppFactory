using System.Windows;
using Template.Config.Content;

namespace Template.Data
{
    public interface ISocialActivityWrapper
    {
        SocialActivity SocialActivity { get; set; }

        bool FullFunctionality { get; set; }

        bool LimitedFunctionality { get; set; }

        bool TrialModeOver { get; set; }

        SocialSource FeedSource { get; set; }


        bool Read { get; set; }

        Visibility DescriptionVisibility { get; }

        string FriendlyTimeStamp { get; }

        string ActivitySourceAndTime { get; }

        Visibility TitleVisibility { get; }


        bool NotRead { get; }
        Visibility ReadVisibility { get; }
        Visibility NotReadVisibility { get; }
        Enclosure[] ImageEnclosures { get; }

        Enclosure[] VideoEnclosures { get; }
        bool VideosExist { get; }
        bool ImagesExist { get; }
        Visibility ImageEnclosuresVisibility { get; }

        Visibility VideoEnclosuresVisibility { get; }

        Enclosure FirstImage { get; }

        bool HasShortenedUrl { get; }
    }
}