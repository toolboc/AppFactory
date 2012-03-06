using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using BuiltToRoam;
using BuiltToRoam.Interfaces;
using BuiltToRoam.Social;
using Template.Config;
using Template.Config.Ads;
using Template.Config.Content.Lists;
using Template.Data;

namespace Template
{
    public interface IRepository : IBaseRepository
    {

        bool IsUpdating { get; }
        event EventHandler IsUpdatingChanged;
        bool IsLoading { get; }
        event EventHandler IsLoadingChanged;
        ReaderConfiguration Configuration { get; set; }
        event EventHandler<ParameterEventArgs<ISocialActivityWrapper>> SocialFeedLoaded;
        ObservableCollection<ISocialActivityWrapper> Activities { get; }
        int MaximumActivitiesToCache { get; set; }
        int UpdateThresholdInMilliseconds { get; set; }
        void Refresh(params string[] onDemandSourcesByName);
        void LoadMore(params string[] onDemandSourcesByName);

        List<FilteredActivityList> FilteredLists { get; }
        PageAdInformation PageAd(Type pageType);

        DateTime FirstRun { get; }
        DateTime TrialExpiresOn { get; }
        DateTime FullFunctionalityTrialExpiresOn { get; }
        bool IsInFullFunctionalityTrialPeriod { get; }
        bool IsInLimitedFunctionalityTrialPeriod { get; }
        bool IsInTrialPeriod { get; }
        bool IsInFullFunctionalityMode { get; }
        bool IsInLimitedFunctionalityMode { get; }
        bool IsInTrialExpiredMode { get; }
        bool IsInTrialMode { get; }

        Visibility FullFunctionalityVisibility { get; }

        Visibility LimitedFunctionalityVisibility { get; }

        Visibility TrialPeriodOverVisibility { get; }

        ISocial UrlShortener { get; }
        bool IsShortenerAvailable { get; }

        void SaveActivities();

        bool CachedActivitiesLoaded { get; }
        void PinActivityListToStart(string listName);
        void UnPinActivityListToStart(string listName);
        bool IsPinned(string listName);
    }

    public class PageAdInformation
    {
        public Visibility AdsVisibility { get { return (Provider != null & Unit != null).ToVisibility(); } }
        public IAdProvider Provider { get; set; }
        public IAdUnit Unit { get; set; }
    }
}
