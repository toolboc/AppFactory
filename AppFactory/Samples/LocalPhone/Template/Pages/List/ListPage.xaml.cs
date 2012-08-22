using System;
using System.Windows.Controls;
using BuiltToRoam;
using Template.Data;

namespace Template.Pages.List
{
    public partial class ListPage
    {
        public ListPage()
        {
            InitializeComponent();

            var pageAd = ViewModel<ListPageViewModel>().PageAd;
            PageHelper.BuildAdControl(pageAd, AdPlaceholder);
        }

        private void RefreshClick(object sender, EventArgs e)
        {
            ViewModel<ListPageViewModel>().RefreshList();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sender.DoActionWithListBoxSelection<SocialActivityWrapper>(ViewModel<ListPageViewModel>().DisplayActivity);
        }

        private void HomeClick(object sender, EventArgs e)
        {
            ViewModel<ListPageViewModel>().DisplayMainPage();
        }
    }
}