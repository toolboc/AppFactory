using BuiltToRoam.BaseClasses;
using Template.Config;
using Template.Data;
using Template.Pages.About;
using Template.Pages.Auth;
using Template.Pages.List;
using Template.Pages.Main;
using Template.Pages.Pivot;
using Template.Pages.Post;
using Template.Pages.Reader;
using Template.Pages.Settings;

namespace Template
{
    public class ViewModelLocator : TypedLocatorBase<RunTimeRepository, RunTimeRepository>
    {
        /// <summary>
        /// Returns a new instance of the Main page view model
        /// </summary>
        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return CurrentViewModel<MainPageViewModel>();
            }
        }

        /// <summary>
        /// Returns a new instance of the reader page view model
        /// </summary>
        public ReaderPageViewModel ReaderPageViewModel
        {
            get
            {
                return CurrentViewModel<ReaderPageViewModel>();
            }
        }

        /// <summary>
        /// Returns a new instance of the settings page view model
        /// </summary>
        public SettingsPageViewModel SettingsPageViewModel
        {
            get
            {
                return CurrentViewModel<SettingsPageViewModel>();
            }
        }

        /// <summary>
        /// Returns a new instance of the about page view model
        /// </summary>
        public AboutPageViewModel AboutPageViewModel
        {
            get
            {
                return CurrentViewModel<AboutPageViewModel>();
            }
        }

        public PivotPageViewModel PivotPageViewModel
        {
            get { return CurrentViewModel<PivotPageViewModel>(); }
        }

        public AuthPageViewModel AuthPageViewModel
        {
            get { return CurrentViewModel<AuthPageViewModel>(); }
        }

        public PostPageViewModel PostPageViewModel
        {
            get { return CurrentViewModel<PostPageViewModel>(); }
        }

        public ListPageViewModel ListPageViewModel
        {
            get { return CurrentViewModel<ListPageViewModel>(); }
        }

        public ReaderConfiguration Configuration
        {
            get
            {
                var repository = Repository as IRepository;
                if (repository == null) return null;
                return repository.Configuration;
            }
            set
            {
                var repository = Repository as IRepository;
                if (repository == null) return;
                repository.Configuration = value;
            }
        }


    }
}
