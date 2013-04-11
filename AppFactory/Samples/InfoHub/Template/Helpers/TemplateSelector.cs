using InfoHub.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace InfoHub.Helpers
{
    public class TemplateSelector: DataTemplateSelector
    {
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            if (item is NewsArticle)
                return (item as IArticle).Hero ? NewsTemplateHero ?? NewsTemplateNormal : NewsTemplateNormal;
            if (item is FlickrArticle)
                return (item as IArticle).Hero ? FlickrTemplateHero ?? FlickrTemplateNormal : FlickrTemplateNormal;
            if (item is TwitterArticle)
                return (item as IArticle).Hero ? TwitterTemplateHero ?? TwitterTemplateNormal : TwitterTemplateNormal;
            if (item is AdvertArticle)
                return (item as IArticle).Hero ? AdvertTemplateHero ?? AdvertTemplateNormal : AdvertTemplateNormal;
            if (item is CalendarArticle)
                return (item as IArticle).Hero ? CalendarTemplateHero ?? CalendarTemplateNormal : CalendarTemplateNormal;
            if (item is YouTubeArticle)
                return (item as IArticle).Hero ? YouTubeTemplateHero ?? YouTubeTemplateNormal : YouTubeTemplateNormal;
            if (item is WeatherArticle)
                return (item as IArticle).Hero ? WeatherTemplateHero ?? WeatherTemplateNormal : WeatherTemplateNormal;
            if (item is SummaryArticle)
                return (item as IArticle).Hero ? SummaryTemplateHero ?? SummaryTemplateNormal : SummaryTemplateNormal;

            // fallback
            return base.SelectTemplateCore(item, container);
        }

        public Windows.UI.Xaml.DataTemplate NewsTemplateHero { get; set; }
        public Windows.UI.Xaml.DataTemplate AdvertTemplateHero { get; set; }
        public Windows.UI.Xaml.DataTemplate FlickrTemplateHero { get; set; }
        public Windows.UI.Xaml.DataTemplate TwitterTemplateHero { get; set; }
        public Windows.UI.Xaml.DataTemplate CalendarTemplateHero { get; set; }
        public Windows.UI.Xaml.DataTemplate SummaryTemplateHero { get; set; }
        public Windows.UI.Xaml.DataTemplate YouTubeTemplateHero { get; set; }
        public Windows.UI.Xaml.DataTemplate WeatherTemplateHero { get; set; }

        public Windows.UI.Xaml.DataTemplate NewsTemplateNormal { get; set; }
        public Windows.UI.Xaml.DataTemplate AdvertTemplateNormal { get; set; }
        public Windows.UI.Xaml.DataTemplate FlickrTemplateNormal { get; set; }
        public Windows.UI.Xaml.DataTemplate TwitterTemplateNormal { get; set; }
        public Windows.UI.Xaml.DataTemplate CalendarTemplateNormal { get; set; }
        public Windows.UI.Xaml.DataTemplate SummaryTemplateNormal { get; set; }
        public Windows.UI.Xaml.DataTemplate YouTubeTemplateNormal { get; set; }
        public Windows.UI.Xaml.DataTemplate WeatherTemplateNormal { get; set; }
    }
}
