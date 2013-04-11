using System.Linq;
using System.Windows;
using BuiltToRoam.WindowsPhone.Controls;
using InfoHubPhone8.Config.Implementations;
using InfoHubPhone8.Config.Structure;
using InfoHubPhone8.Pages.Main;

namespace InfoHubPhone8.UI
{
    /// <summary>
    /// This class inherits from ContentControl and can be used
    /// to display different reading layouts based on the specified
    /// template name. Databind a ReadingPageImplementation to the Content 
    /// property and add the different reading layout templates as Resources
    /// on the control. The template name is determined by looking up the
    /// source type name of the activity being displayed. If no match is 
    /// found the default template name is used.
    /// </summary>
    public class ReadingLayoutTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var impl = item as ReaderPageImplementation;
            if (impl == null) return base.SelectTemplate(item, container);

            var def = impl.Definition as ReaderPageDefinition;

            var sourceType = impl.Activity.FeedSource.SourceTypeName;

            // Find the template name based on the social type name
            if (def != null)
            {
                var templateName = (from template in def.ReaderTemplates
                                    where template.SocialTypeName == sourceType
                                    select template).FirstOrDefault() ?? (from template in def.ReaderTemplates
                                                                          where string.IsNullOrWhiteSpace(template.SocialTypeName)
                                                                          select template).FirstOrDefault();

                if (templateName == null) return base.SelectTemplate(item, container);

                var dataT = Resources[templateName.TemplateName] as DataTemplate;
                if (dataT != null) return dataT;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
