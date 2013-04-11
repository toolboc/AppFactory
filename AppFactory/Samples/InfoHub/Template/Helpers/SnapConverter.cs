using InfoHub.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace InfoHub.Helpers
{
    public class SnapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var _Item = value as GroupedItem;
            if (!_Item.Articles
                .Where(x => !(x is AdvertArticle))
                .Where(x => !(x is SummaryArticle))
                .Where(x => !(x is CalendarArticle))
                .Where(x => !(x is WeatherArticle))
                .Any())
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
