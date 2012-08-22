using System;
using System.Collections.Generic;
using System.Windows;
using BuiltToRoam;
using BuiltToRoam.Interfaces;
using Template.Config.Content;

namespace Template.Pages.About
{
    public class AboutPageViewModel : SocialReaderViewModelBase<DefaultPageStates, DefaultAppBarButtons>
    {
        /// <summary>
        /// Initializes a new instance of the AboutPageViewModel class
        /// </summary>
        /// <param name="repository"></param>
        public AboutPageViewModel(IRepository repository, IDispatcher dispatcher, INavigation navigation)
            : base(repository, dispatcher, navigation)
        {
        }

        /// <summary>
        /// Returns information about the trial mode
        /// </summary>
        public string TrialMode
        {
            get
            {
                string tm;
                if (!this.IsInTrialMode()) return string.Empty;
                tm = "trial period ";
                if (Repository.IsInFullFunctionalityTrialPeriod)
                {
                    var days = Repository.FullFunctionalityTrialExpiresOn.Date.Subtract(DateTime.Now.Date).Days;
                    return "(full feature)" + tm + "expires in " + days + (days > 1 ? " days" : " day");
                }
                if (Repository.IsInTrialPeriod)
                {
                    if (Repository.TrialExpiresOn < DateTime.MaxValue)
                    {
                        var days = Repository.TrialExpiresOn.Date.Subtract(DateTime.Now.Date).Days;
                        return tm + "expires in " + days + (days > 1 ? " days" : " day");
                    }
                }

                return tm + "is over";
            }

        }

        /// <summary>
        /// Returns the list of data sources
        /// </summary>
        public List<SocialSource> DataSources
        {
            get { return Repository.Configuration.Sources; }
        }

        /// <summary>
        /// Returns whether the application has been purchased or is in trial mode
        /// </summary>
        public Visibility PurchaseVisibility
        {
            get { return IsInTrialModeVisibility; }
        }

        /// <summary>
        /// Returns whether the feedback button should be displayed or not
        /// </summary>
        public Visibility FeedbackVisibility
        {
            get { return (!string.IsNullOrEmpty(Repository.Configuration.FeedbackEmailAddress)).ToVisibility(); }
        }

        /// <summary>
        /// Returns the application version information
        /// </summary>
        public string ApplicationVersion
        {
            get
            {
                return "Version: " + Utilities.ApplicationVersion;
            }
        }

        /// <summary>
        /// Returns the application publisher 
        /// </summary>
        public string ApplicationPublisher
        {
            get
            {
                return "Publisher: " + Utilities.ApplicationPublisher;
            }
        }

        /// <summary>
        /// Launches the feedback via email
        /// </summary>
        public void LaunchFeedback()
        {
            Utilities.LaunchEmailCompose(Repository.Configuration.FeedbackEmailAddress, Utilities.ApplicationTitle + " Feedback");
        }
    }
}
