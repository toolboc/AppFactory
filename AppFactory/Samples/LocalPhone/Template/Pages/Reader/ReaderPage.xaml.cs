using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BuiltToRoam;
using BuiltToRoam.WindowsPhone.Controls;
using Microsoft.Phone.Controls;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Template.Pages.Reader
{
    public partial class ReaderPage
    {

        // Constructor
        public ReaderPage()
        {
            InitializeComponent();

            var pageAd = ViewModel<ReaderPageViewModel>().PageAd;
            PageHelper.BuildAdControl(pageAd, AdPlaceholder);

        }

        /// <summary>
        /// Handles the link button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LinkButtonClick(object sender, EventArgs e)
        {
            ViewModel<ReaderPageViewModel>().ShowLinkList();
        }

        /// <summary>
        /// Handles the comment menu item click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentClick(object sender, EventArgs e)
        {
            ViewModel<ReaderPageViewModel>().Comment();
        }

        /// <summary>
        /// Handles the post menu item click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostClick(object sender, EventArgs e)
        {
            ViewModel<ReaderPageViewModel>().Post();
        }

        /// <summary>
        /// Handles the repost menu item click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RepostClick(object sender, EventArgs e)
        {
            ViewModel<ReaderPageViewModel>().Repost();
        }

        /// <summary>
        /// Handles the reply menu item click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplyClick(object sender, EventArgs e)
        {
            ViewModel<ReaderPageViewModel>().Reply();
        }

        /// <summary>
        /// Handles the reply all menu item click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplyAllClick(object sender, EventArgs e)
        {
            ViewModel<ReaderPageViewModel>().ReplyAll();
        }

        /// <summary>
        /// Handles the email menu item click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmailClick(object sender, EventArgs e)
        {
            ViewModel<ReaderPageViewModel>().Email();
        }

        /// <summary>
        /// Handles the previous button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousClick(object sender, EventArgs e)
        {
            ResetContentScroll();
            ViewModel<ReaderPageViewModel>().Previous();
        }

        /// <summary>
        /// Handles the next button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextClick(object sender, EventArgs e)
        {
            ResetContentScroll();
            ViewModel<ReaderPageViewModel>().Next();
        }

        private void ResetContentScroll()
        {
            var scroll = ReaderContent.FindControlByType<ScrollViewer>();
            scroll.DoIfNotNull(s => s.ScrollToVerticalOffset(0));
        }


        /// <summary>
        /// Handle the user clicking the Author TextBlock
        /// </summary>
        private void AuthorTap(object sender, GestureEventArgs eventArgs)
        {
            ViewModel<ReaderPageViewModel>().OpenAuthor();
        }

        /// <summary>
        /// Handles the open in browser menu item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenInBrowserClick(object sender, EventArgs e)
        {
            ViewModel<ReaderPageViewModel>().OpenInBrowser();
        }

        /// <summary>
        /// Handles when the user taps on the play video link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoTap(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var source = sender as FrameworkElement;
            if (source == null) return;
            var url = source.Tag as string;
            if (string.IsNullOrEmpty(url)) return;

            // Extract the you tube videourl
            var youtube = ExtractYouTubeVideoUrl(url);
            Utilities.DisplayInWebBrowser(!string.IsNullOrEmpty(youtube) ? youtube : url);
        }

        /// <summary>
        /// Extracts the you tube video url from the url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string ExtractYouTubeVideoUrl(string url)
        {
            // Look for the full youtube domain
            if (url.ToLower().Contains("youtube"))
            {
                var idx = url.IndexOf('?');
                if (idx >= 0)
                {
                    var query = url.Substring(idx + 1);
                    var pairs = query.Split('&');
                    foreach (var pair in pairs)
                    {
                        var bits = pair.Split('=');
                        if (bits.Length == 2 && bits[0].ToLower() == "v")
                        {
                            var key = string.Format("vnd.youtube:{0}", bits[1]);
                            return key;
                        }

                    }
                }
            }

            // Look for the shortened youtu.be domain
            if (url.ToLower().Contains("youtu.be"))
            {
                var idx = url.IndexOf("/", 8, StringComparison.Ordinal); // http:// <-- That's 7 characters, so skip 8 
                if (idx >= 0)
                {
                    var key = url.Substring(idx + 1);
                    key = string.Format("vnd.youtube:{0}", key);
                    return key;
                }
            }
            return null;
        }

        /// <summary>
        /// Handle the content parsed event on the main htmltextblock so that the list of links can be retrieved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentParsed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                                       {
                                           var htmlTextBlock = sender as HtmlTextBlock;
                                           if (htmlTextBlock != null)
                                           {
                                               ViewModel<ReaderPageViewModel>().Links = htmlTextBlock.Links;
                                           }
                                       });
        }

        /// <summary>
        /// Handle when a link is selected - and display it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var link = listBox.SelectedItem as Link;
                if (link == null) return;
                listBox.SelectedIndex = -1;
                ViewModel<ReaderPageViewModel>().DisplayLink(link);
            }
        }

        private WebBrowser browser;
        private void WireUpWebBrowser(object sender, RoutedEventArgs e)
        {
            ViewModel<ReaderPageViewModel>().PropertyChanged += WebContentChanging;
            browser = sender as WebBrowser;
            if (browser != null) browser.Navigating += browser_Navigating;
            LoadWebContent();
        }

        private void LoadWebContent()
        {

            var activity = ViewModel<ReaderPageViewModel>().Activity;
            var bc = FetchBackgroundColor();

            var fc = FetchFontColor();

            var webcontent = "<HTML>" +
"<HEAD>" +
"<meta name=\"viewport\" content=\"width=320\" />" +
"</HEAD>" +
"<BODY style=\"background-color:" + bc + ";color:" + fc + "\">" +
activity.SocialActivity.Description +
"</BODY>" +
"</HTML>";

            browser.NavigateToString(webcontent);

        }

        private void WebContentChanging(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Activity")
            {
                LoadWebContent();
            }
        }

        void browser_Navigating(object sender, NavigatingEventArgs e)
        {
            e.Cancel = true;
            if (e.Uri.IsAbsoluteUri && (e.Uri.Scheme.ToLower() == "http" || e.Uri.Scheme.ToLower() == "https"))
            {
                Utilities.DisplayInWebBrowser(e.Uri.OriginalString);
            }
        }



        private string FetchBackgroundColor()
        {
            return IsBackgroundBlack() ? "#000;" : "#fff";
        }

        private string FetchFontColor()
        {
            return IsBackgroundBlack() ? "#fff;" : "#000";
        }

        private static bool IsBackgroundBlack()
        {
            return FetchBackGroundColor() == "#FF000000";
        }

        private static string FetchBackGroundColor()
        {
            var mc = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            string color = mc.ToString();
            return color;
        }

    }
}