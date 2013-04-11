using InfoHub.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InfoHub.Helpers
{
    public class SnapListView : ListView
    {
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
        {
            if (item is AdvertArticle)
            {
                var _UiElement = element as UIElement;
                if (_UiElement != null) _UiElement.Visibility = Visibility.Collapsed;
            }
            if (item is SummaryArticle)
            {
                var _Element = element as UIElement;
                if (_Element != null) _Element.Visibility = Visibility.Collapsed;
            }
            if (item is CalendarArticle)
            {
                var _UiElement1 = element as UIElement;
                if (_UiElement1 != null) _UiElement1.Visibility = Visibility.Collapsed;
            }
            if (item is WeatherArticle)
            {
                var _Element1 = element as UIElement;
                if (_Element1 != null) _Element1.Visibility = Visibility.Collapsed;
            }
            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
