using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BuiltToRoam;
using Microsoft.Phone.Shell;
using BuiltToRoam.WindowsPhone.Controls;
using Microsoft.Phone.Controls;
using InfoHubPhone8.Config.Implementations;
using InfoHubPhone8.Config.Structure;
using InfoHubPhone8.Data;
using InfoHubPhone8.UI;

namespace InfoHubPhone8.Pages.Main
{
    public partial class MainPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle selection changed from any of the listboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenSelectedListItem(sender as ListBox);
        }


        /// <summary>
        /// Open the selected item in the reader page
        /// </summary>
        /// <param name="list"></param>
        private void OpenSelectedListItem(ListBox list)
        {
            // Grab the selected item and check to make sure that something is selected
            var item = list.SelectedItem as ISocialActivityWrapper;
            if (item != null)
            {
                // Reset the selected item so that when the user comes back to this page
                // there is nothing selected in the list (otherwise selecting the same 
                // item again won't trigger the selection changed event).
                list.SelectedIndex = -1;

                IEnumerable<ISocialActivityWrapper> source = list.ItemsSource as IEnumerable<ISocialActivityWrapper>;
                var lazy = list as LazyListBox;
                if (lazy != null)
                {
                    source = lazy.Source as IEnumerable<ISocialActivityWrapper>;
                }

                // Display the item
                ViewModel<MainPageViewModel>().DisplaySocialActivity(item, source);

            }
        }


        /// <summary>
        /// Handle the refresh button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshClick(object sender, System.EventArgs e)
        {
            ViewModel<MainPageViewModel>().RefreshActivities();
        }


        /// <summary>
        /// Handles the list scroll event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListScrolled(object sender, ParameterEventArgs<double> e)
        {
            // When the scrolling gets over 90%, go fetch the next set of activities
            if (e.Parameter1 > 90)
            {
                ViewModel<MainPageViewModel>().MoreActivities();
            }
        }

        private void LinkMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var link = sender.SelectedItem<LinkDefinition>();
            if (link == null) return;


            ViewModel<MainPageViewModel>().LaunchLink(link);

        }

        private void PanoramaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var paneDefinition = MainPanorama.SelectedItem as PaneImplementation;
            ViewModel<MainPageViewModel>().ChangePanoramaSelection(paneDefinition);
        }

        private void ApplicationBarStateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            ApplicationBar.Opacity = e.IsMenuVisible ? 1 : 0;
        }

        private void SettingsClick(object sender, EventArgs e)
        {
            ViewModel<MainPageViewModel>().DisplaySettings();
        }

        private void ListLoaded(object sender, RoutedEventArgs e)
        {
            (sender as LazyListBox).Scrolled += ListScrolled;
        }

        private void pinClick(object sender, EventArgs e)
        {
            var paneDefinition = MainPanorama.SelectedItem as PaneImplementation;
            ViewModel<MainPageViewModel>().PinList(paneDefinition);
        }

        private void unPinClick(object sender, EventArgs e)
        {
            var paneDefinition = MainPanorama.SelectedItem as PaneImplementation;
            ViewModel<MainPageViewModel>().UnPinList(paneDefinition);
        }
    }
}