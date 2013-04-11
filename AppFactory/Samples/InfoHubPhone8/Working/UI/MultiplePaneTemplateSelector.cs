using System.Linq;
using System.Windows;
using BuiltToRoam.WindowsPhone.Controls;
using Microsoft.Phone.Controls;
using InfoHubPhone8.Config.Implementations;
using InfoHubPhone8.Config.Structure;
using InfoHubPhone8.Pages.Main;

namespace InfoHubPhone8.UI
{
    /// <summary>
    /// This class inherits from ContentControl and can be used
    /// to display different pane (eg  layouts based on the specified
    /// template name. Databind a PaneImplementation to the Content 
    /// property and then specify the ListTemplate and LinkTemplate via
    /// the exposed properties
    /// </summary>
    public class MultiplePaneTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ListTemplate
        {
            get;
            set;
        }

        public DataTemplate LinkTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var pi = item as PaneImplementation;
            if (pi != null && pi.Definition is ListPaneDefinition)
            {
                var width = (pi.Definition as ListPaneDefinition).MaxWidth;
                if (width > 0)
                {
                    var panoitem = container.Ancestors().OfType<PanoramaItem>().FirstOrDefault();
                    panoitem.Orientation = (pi.Definition as ListPaneDefinition).Orientation;
                    panoitem.MaxWidth = width;
                }

                return ListTemplate;
            }

            if (pi.Definition != null && pi.Definition is LinkPaneDefinition)
            {
                return LinkTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}