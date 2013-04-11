using System.Windows;
using System.Windows.Markup;
using BuiltToRoam;
using BuiltToRoam.BaseClasses;

namespace InfoHubPhone8.Config.Structure
{
    [ContentProperty("Destination")]
    public class LinkDefinition : NotifyBase
    {
        public string Title { get; set; }

        public string LinkType { get; set; }

        public LinkType Type
        {
            get
            {
                return LinkType.EnumParse<LinkType>();
            }
        }

        public string Destination { get; set; }

        public string Parameter { get; set; }

        private Visibility _Visibility = System.Windows.Visibility.Visible;

        public Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                if (Visibility == value) return;
                _Visibility = value;
                RaisePropertyChanged(() => Visibility);
            }
        }
    }
}