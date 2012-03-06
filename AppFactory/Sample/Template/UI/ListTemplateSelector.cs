using System.Windows;
using BuiltToRoam.WindowsPhone.Controls;
using Template.Config.Implementations;
using Template.Config.Structure;

namespace Template.UI
{
    /// <summary>
    /// This class inherits from ContentControl and can be used
    /// to display different lists layouts based on the specified
    /// template name. Databind a PaneImplementation to the Content 
    /// property and add the different list templates as Resources
    /// on the control
    /// </summary>
    public class ListTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var pi = item as PaneImplementation;
            if (pi != null)
            {
                var def = pi.Definition as ListPaneDefinition;
                if (def == null) return base.SelectTemplate(item, container);

                var template = Resources[def.TemplateName] as DataTemplate;
                if (template != null) return template;
            }

            return base.SelectTemplate(item, container);
        }
    }
}