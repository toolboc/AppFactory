using System;
using System.Windows.Controls;

namespace InfoHubPhone8.Pages.Post
{
    public partial class PostPage
    {
        public PostPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles changes in the text box - to make sure that the binding
        /// is always up to date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextToPostChanged(object sender, TextChangedEventArgs e)
        {
            // Force update binding when text change - otherwise the data source
            // won't be updated if the user clicks the application bar
            var binding = (sender as TextBox).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }

        private void SubmitClick(object sender, EventArgs e)
        {
            //ViewModel<PostPageViewModel>().Post();
        }
    }
}