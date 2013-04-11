﻿//      *********    DO NOT MODIFY THIS FILE     *********
//      This file is regenerated by a design tool. Making
//      changes to this file can cause errors.
namespace Expression.Blend.SampleData.ActivitySampleData
{
    using System;

    // To significantly reduce the sample data footprint in your production application, you can set
    // the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#if DISABLE_SAMPLE_DATA
	internal class ActivitySampleData { }
#else

    public class ActivitySampleData : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        public ActivitySampleData()
        {
            try
            {
                System.Uri resourceUri = new System.Uri("/InfoHubPhone8;component/SampleData/ActivitySampleData/ActivitySampleData.xaml", System.UriKind.Relative);
                if (System.Windows.Application.GetResourceStream(resourceUri) != null)
                {
                    System.Windows.Application.LoadComponent(this, resourceUri);
                }
            }
            catch (System.Exception)
            {
            }
        }

        private Activities _Activities = new Activities();

        public Activities Activities
        {
            get
            {
                return this._Activities;
            }
        }

        private bool _TrialModeOver = false;

        public bool TrialModeOver
        {
            get
            {
                return this._TrialModeOver;
            }

            set
            {
                if (this._TrialModeOver != value)
                {
                    this._TrialModeOver = value;
                    this.OnPropertyChanged("TrialModeOver");
                }
            }
        }
    }

    public class Activities : System.Collections.ObjectModel.ObservableCollection<ActivitiesItem>
    {
    }

    public class ActivitiesItem : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private SocialActivity _SocialActivity = new SocialActivity();

        public SocialActivity SocialActivity
        {
            get
            {
                return this._SocialActivity;
            }

            set
            {
                if (this._SocialActivity != value)
                {
                    this._SocialActivity = value;
                    this.OnPropertyChanged("SocialActivity");
                }
            }
        }

        private ImageEnclosures1 _ImageEnclosures = new ImageEnclosures1();

        public ImageEnclosures1 ImageEnclosures
        {
            get
            {
                return this._ImageEnclosures;
            }
        }

        private FirstImage _FirstImage = new FirstImage();

        public FirstImage FirstImage
        {
            get
            {
                return this._FirstImage;
            }

            set
            {
                if (this._FirstImage != value)
                {
                    this._FirstImage = value;
                    this.OnPropertyChanged("FirstImage");
                }
            }
        }

        private bool _ImagesExist = false;

        public bool ImagesExist
        {
            get
            {
                return this._ImagesExist;
            }

            set
            {
                if (this._ImagesExist != value)
                {
                    this._ImagesExist = value;
                    this.OnPropertyChanged("ImagesExist");
                }
            }
        }

        private string _ActivitySourceAndTime = string.Empty;

        public string ActivitySourceAndTime
        {
            get
            {
                return this._ActivitySourceAndTime;
            }

            set
            {
                if (this._ActivitySourceAndTime != value)
                {
                    this._ActivitySourceAndTime = value;
                    this.OnPropertyChanged("ActivitySourceAndTime");
                }
            }
        }
    }

    public class SocialActivity : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _NotRead = false;

        public bool NotRead
        {
            get
            {
                return this._NotRead;
            }

            set
            {
                if (this._NotRead != value)
                {
                    this._NotRead = value;
                    this.OnPropertyChanged("NotRead");
                }
            }
        }

        private string _Author = string.Empty;

        public string Author
        {
            get
            {
                return this._Author;
            }

            set
            {
                if (this._Author != value)
                {
                    this._Author = value;
                    this.OnPropertyChanged("Author");
                }
            }
        }

        private string _Title = string.Empty;

        public string Title
        {
            get
            {
                return this._Title;
            }

            set
            {
                if (this._Title != value)
                {
                    this._Title = value;
                    this.OnPropertyChanged("Title");
                }
            }
        }

        private string _ActivitySourceAndTime = string.Empty;

        public string ActivitySourceAndTime
        {
            get
            {
                return this._ActivitySourceAndTime;
            }

            set
            {
                if (this._ActivitySourceAndTime != value)
                {
                    this._ActivitySourceAndTime = value;
                    this.OnPropertyChanged("ActivitySourceAndTime");
                }
            }
        }

        private bool _Read = false;

        public bool Read
        {
            get
            {
                return this._Read;
            }

            set
            {
                if (this._Read != value)
                {
                    this._Read = value;
                    this.OnPropertyChanged("Read");
                }
            }
        }

        private string _Description = string.Empty;

        public string Description
        {
            get
            {
                return this._Description;
            }

            set
            {
                if (this._Description != value)
                {
                    this._Description = value;
                    this.OnPropertyChanged("Description");
                }
            }
        }

        private string _FeedSourceName = string.Empty;

        public string FeedSourceName
        {
            get
            {
                return this._FeedSourceName;
            }

            set
            {
                if (this._FeedSourceName != value)
                {
                    this._FeedSourceName = value;
                    this.OnPropertyChanged("FeedSourceName");
                }
            }
        }
    }

    public class ImageEnclosures1 : System.Collections.ObjectModel.ObservableCollection<ImageEnclosuresItem1>
    {
    }

    public class ImageEnclosuresItem1 : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private System.Windows.Media.ImageSource _IconUrl = null;

        public System.Windows.Media.ImageSource IconUrl
        {
            get
            {
                return this._IconUrl;
            }

            set
            {
                if (this._IconUrl != value)
                {
                    this._IconUrl = value;
                    this.OnPropertyChanged("IconUrl");
                }
            }
        }
    }

    public class FirstImage : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private System.Windows.Media.ImageSource _IconUrl = null;

        public System.Windows.Media.ImageSource IconUrl
        {
            get
            {
                return this._IconUrl;
            }

            set
            {
                if (this._IconUrl != value)
                {
                    this._IconUrl = value;
                    this.OnPropertyChanged("IconUrl");
                }
            }
        }
    }
#endif
}
