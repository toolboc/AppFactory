
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BuiltToRoam;
using BuiltToRoam.WindowsPhone.Controls;
using Microsoft.Advertising.Mobile.UI;
using InfoHubPhone8.Config.Content.Lists;
using Microsoft.Phone.Controls;
using InfoHubPhone8.Config.Implementations;
using InfoHubPhone8.Data;
using InfoHubPhone8.Pages.Main;
using InfoHubPhone8.UI;

namespace InfoHubPhone8.Pages.Pivot
{
    public partial class PivotPage
    {
        // Constructor
        public PivotPage()
        {
            InitializeComponent();


            var pageAd = ViewModel<PivotPageViewModel>().PageAd;
            PageHelper.BuildAdControl(pageAd, AdPlaceholder);
        }



        private void LoadingPivotItemHandler(object sender, PivotItemEventArgs e)
        {
            var list = sender as Microsoft.Phone.Controls.Pivot;
            if (list == null) return;
            var selected = list.SelectedItem as ListPaneImplementation;
            if (selected == null) return;


            (DataContext as PivotPageViewModel).DownloadPivotContent(selected.List);


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
                (this.DataContext as PivotPageViewModel).DisplaySocialActivity(item, source);

            }
        }

        private void ListLoaded(object sender, RoutedEventArgs e)
        {
            (sender as LazyListBox).Scrolled += ListScrolled;
        }


        /// <summary>
        /// Handles the list scroll event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListScrolled(object sender, ParameterEventArgs<double> e)
        {
            // Assumption, that when the user is scrolling they're on the current pivot
            var pivot = (sender as LazyListBox).DataContext as FilteredActivityList;
            if (pivot == null) return;
            // When the scrolling gets over 90%, go fetch the next set of activities
            if (e.Parameter1 > 90)
            {
                (this.DataContext as PivotPageViewModel).MoreActivities(pivot);
            }
        }

        private void RefreshClick(object sender, EventArgs e)
        {
            // Assumption, that when the user is scrolling they're on the current pivot
            var pivotlist = ListPivot.SelectedItem as FilteredActivityList;
            if (pivotlist == null) return;
            (this.DataContext as PivotPageViewModel).RefreshActivities(pivotlist);
        }

    }
}