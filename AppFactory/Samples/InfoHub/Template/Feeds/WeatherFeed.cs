using InfoHub.Articles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace InfoHub.Feeds
{
    public class WeatherFeed : FeedBase
    {
        public class Values
        {
            public string Date { get; set; }
            public DateTime DateTime { get { return DateTime.Parse(Date); } }
            public int ShortIndex { get; set; }
            public int LongIndex { get; set; }
            public string High { get; set; }
            public string Low { get; set; }
            public string Rain { get; set; }
            public string Summary { get; set; }
            public string Details { get; set; }
            public string Icon { get; set; }
            public WeatherArticle ToWeatherArticle()
            {
                return new WeatherArticle
                {
                    Title = this.Details,
                    Date = this.DateTime,
                    Body = string.Format("{0}/{1}F {2}%", this.High, this.LongIndex, this.Rain),
                    Image = this.Icon,
                    Url = "weather:" + this.DateTime.ToString().GetHashCode().ToString(),
                };
            }
        }

        public async override Task<bool> ReadFromWeb()
        {
            if (this.Busy)
                return false;
            this.Busy = true;

            try
            {

                // including user agent, otherwise FB rejects the request
                var _Client = new HttpClient();
                var _UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                _Client.DefaultRequestHeaders.Add("user-agent", _UserAgent);

                // fetch as string to avoid error
                var _Uri = new Uri(this.SourceUrl);
                var _Response = await _Client.GetAsync(_Uri);
                var _String = await _Response.Content.ReadAsStringAsync();

                // convert to xml (will validate, too)
                var _XmlDocument = new Windows.Data.Xml.Dom.XmlDocument();
                _XmlDocument.LoadXml(_String);

                // parse

                var _Point1 = _XmlDocument.SelectSingleNode("//parameters[@applicable-location='point1']");

                var _Highs = _Point1.SelectNodes("//temperature[@type='maximum']/value")
                    .Select((x, i) => new { ShortIndex = i, Value = x.InnerText });

                var _LowNodes = _Point1.SelectNodes("//temperature[@type='minimum']/value");
                var _Lows = _LowNodes
                    .Select((x, i) => new { ShortIndex = _LowNodes.Count == 6 ? i + 1 : i, Value = x.InnerText });

                var _Rains = _Point1.SelectNodes("//probability-of-precipitation/value")
                    .Select((x, i) => new { LongIndex = i, Value = string.IsNullOrEmpty(x.InnerText) ? "0" : x.InnerText });

                var _Summaries = _Point1.SelectNodes("//weather/weather-conditions/@weather-summary")
                    .Select((x, i) => new { LongIndex = i, Value = x.InnerText });

                var _Descriptions = _Point1.SelectNodes("//wordedForecast/text")
                    .Select((x, i) => new { LongIndex = i, Value = x.InnerText });

                var _Icons = _Point1.SelectNodes("//conditions-icon/icon-link")
                    .Select((x, i) => new { LongIndex = i, Value = x.InnerText });

                var _LongList = _XmlDocument
                    .SelectNodes("//time-layout[layout-key = 'k-p12h-n13-1' or layout-key = 'k-p12h-n14-1' or layout-key = 'k-p12h-n15-1']/start-valid-time")
                    .Select((x, i) => new { Index = i, Period = x.Attributes[0].InnerText, Text = x.InnerText })
                    .Where(x => !x.Period.ToLower().Contains("night"))
                    .Select(x => new Values
                    {
                        Summary = x.Period.Equals("Today") ? DateTime.Now.ToString("dddd") :
                                  x.Period.ToLower().Contains("afternoon") ? DateTime.Now.ToString("dddd") : x.Period,
                        LongIndex = x.Index,
                        Date = x.Text,
                    });
                var _ShortList = _XmlDocument
                    .SelectNodes("//time-layout[layout-key = 'k-p24h-n7-1' or layout-key = 'k-p24h-n7-2' or layout-key = 'k-p24h-n8-1']/start-valid-time")
                    .Where(x => !x.Attributes[0].InnerText.ToLower().Contains("night"))
                    .Select((x, i) => new { Index = i, Period = x.Attributes[0].InnerText, Text = x.InnerText })
                    .Select(x => new Values
                    {
                        Summary = x.Period.Equals("Today") ? DateTime.Now.ToString("dddd") :
                                  x.Period.ToLower().Contains("afternoon") ? DateTime.Now.ToString("dddd") : x.Period,
                        ShortIndex = x.Index,
                        Date = x.Text,
                    });
                var _List =
                    from _Long in _LongList
                    join _Short in _ShortList on _Long.Date equals _Short.Date
                    select new Values
                    {
                        LongIndex = _Long.LongIndex,
                        ShortIndex = _Short.ShortIndex,
                        Date = _Long.Date,
                        High = _Highs.Any(x => x.ShortIndex == _Short.ShortIndex)
                            ? _Highs.First(x => x.ShortIndex == _Short.ShortIndex).Value : "N/A",
                        Low = _Lows.Any(x => x.ShortIndex == _Short.ShortIndex)
                            ? _Lows.First(x => x.ShortIndex == _Short.ShortIndex).Value : "N/A",
                        Rain = _Rains.Any(x => x.LongIndex == _Long.LongIndex)
                            ? _Rains.First(x => x.LongIndex == _Long.LongIndex).Value : "N/A",
                        Summary = _Summaries.Any(x => x.LongIndex == _Long.LongIndex)
                            ? _Summaries.First(x => x.LongIndex == _Long.LongIndex).Value : "N/A",
                        Details = _Descriptions.Any(x => x.LongIndex == _Long.LongIndex)
                            ? _Descriptions.First(x => x.LongIndex == _Long.LongIndex).Value : "N/A",
                        Icon = _Icons.Any(x => x.LongIndex == _Long.LongIndex)
                            ? _Icons.First(x => x.LongIndex == _Long.LongIndex).Value : string.Empty,
                    };

                this.Articles.Clear();
                foreach (var item in _List)
                    this.Articles.Add(item.ToWeatherArticle());
                await base.WriteToCache();
                return true;
            }
            catch
            {
                Debug.WriteLine("Error reading: " + this.SourceUrl);
                System.Diagnostics.Debugger.Break();
                return false;
            }
            finally { this.Busy = false; }
        }
    }
}
