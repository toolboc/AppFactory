using InfoHub.Articles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InfoHub.Feeds
{
    public class CalendarFeed : FeedBase
    {
        public async override Task<bool> ReadFromWeb()
        {
            if (this.Busy)
                return false;
            this.Busy = true;

            try
            {
                Articles.Clear();

                var _Client = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };
                var _Response = await _Client.GetAsync(this.SourceUrl);
                var _String = await _Response.Content.ReadAsStringAsync();

                var _Parse = (Parser)null;
                int _Index = -1;
                var _Lines = Regex.Split(_String, @"\n");
                foreach (var item in _Lines)
                {
                    if (item.StartsWith("BEGIN:VEVENT"))
                        _Parse = new Parser();
                    if (_Parse == null)
                        continue;
                    if (item.StartsWith("DTSTART;"))
                        _Parse.Date = item.Split(':')[1];
                    if (item.StartsWith("DESCRIPTION:"))
                        _Parse.Body = item.Substring("DESCRIPTION:".Length).Replace(@"\n", "<p/>");
                    if (item.StartsWith("LOCATION:"))
                        _Parse.Location = item.Substring("LOCATION:".Length);
                    if (item.StartsWith("SUMMARY:"))
                        _Parse.Subject = item.Substring("SUMMARY:".Length);
                    if (item.StartsWith("URL:"))
                        _Parse.Url = item.Substring("URL:".Length);
                    if (item.StartsWith("END:VEVENT"))
                    {
                        var _Event = _Parse.ToArticle();
                        _Event.Index = _Index++;
                        if (_Event.Url == null)
                            _Event.Url = new Uri(this.MoreUrl + "#" + _Event.Date.ToString("yyMMddhhmmss")).ToString();
                        this.Articles.Add(_Event);
                    }
                }
                this.Updated = DateTime.Now;
                await base.WriteToCache();
                return true;
            }
            catch
            {
                Debug.WriteLine("Error reading/parsing: " + this.SourceUrl);
                System.Diagnostics.Debugger.Break();
                System.Diagnostics.Debugger.Break();
                return false;
            }
            finally
            {
                this.Busy = false;
            }
        }

        public class Parser
        {
            public string Subject { get; set; }
            public string Date { get; set; }
            public string Url { get; set; }
            public string Author { get; set; }
            public string Body { get; set; }
            public string Location { get; set; }
            public Articles.CalendarArticle ToArticle()
            {
                var _Event = new Articles.CalendarArticle
                {
                    Title = this.Subject,
                    Body = this.Body,
                    Author = this.Location,
                };

                Uri _Uri;
                if (Uri.TryCreate(this.Url, UriKind.Absolute, out _Uri))
                    _Event.Url = _Uri.ToString();

                try
                {
                    var _Year = int.Parse(this.Date.Substring(1 - 1, 4));
                    var _Month = int.Parse(this.Date.Substring(5 - 1, 2));
                    var _Day = int.Parse(this.Date.Substring(7 - 1, 2));
                    var _Date = new DateTime(_Year, _Month, _Day);
                    _Event.Date = _Date;
                }
                catch { }

                return _Event;
            }
        }
    }
}
