using System.Windows;
using BuiltToRoam;
using BuiltToRoam.Interfaces;

namespace InfoHubPhone8.Pages.Post
{
    public class PostPageViewModel : SocialReaderViewModelBase<DefaultPageStates, DefaultAppBarButtons>
    {
        public enum PostType
        {
            Post,
            Repost,
            Reply,
            ReplyAll,
            Comment
        }

        public const string PostTypeKey = "PostType";
        public const string PostMessageKey = "PostMessage";
        public const string PostTitleKey = "PostTitle";

        ///// <summary>
        ///// Whether the number of characters remaining should be displayed
        ///// </summary>
        //private Visibility charactersRemainingVisibility = Visibility.Visible;

        ///// <summary>
        ///// The number of characters remaining (for tweets)
        ///// </summary>
        //private int charactersRemaining = 140;

        ///// <summary>
        ///// The text to be posted
        ///// </summary>
        //private string textToPost;

        //private string postTitle;

        //public string PostTitle
        //{
        //    get { return postTitle; }
        //    set
        //    {
        //        if (PostTitle == value) return;
        //        postTitle = value;
        //        RaisePropertyChanged(() => PostTitle);
        //    }
        //}




        //public Visibility CharactersRemainingVisibility
        //{
        //    get { return charactersRemainingVisibility; }
        //    set
        //    {
        //        if (CharactersRemainingVisibility == value) return;
        //        charactersRemainingVisibility = value;
        //        RaisePropertyChanged(() => CharactersRemainingVisibility);
        //    }
        //}
        ///// <summary>
        ///// Gets/Sets the current post action type
        ///// </summary>
        //private PostType PostAction { get; set; }

        ///// <summary>
        ///// Post to the relevant social network
        ///// </summary>
        //public void Post()
        //{
        //    HideTextEntry();
        //    switch (PostAction)
        //    {
        //        //case PostType.Comment:
        //        //    FacebookHelper.CommentOnPost(SecuritySettings.Instance.FacebookUserInfo, Activity.SocialActivity.Id, TextToPost, PostCommentComplete);
        //        //    break;
        //        //case PostType.Share:
        //        //    FacebookHelper.PostToWall(SecuritySettings.Instance.FacebookUserInfo, TextToPost, PostToWallComplete);
        //        //    break;
        //        case PostType.Reply:
        //        case PostType.ReplyAll:
        //        case PostType.Retweet:
        //        case PostType.Tweet:
        //            // TODO: Posting status update
        //            //TwitterHelper.PostStatusUpdate(SecuritySettings.Instance.TwitterUserInfo, TextToPost, PostTweetComplete);
        //            break;
        //    }

        //}


        ///// <summary>
        ///// Callback for post to facebook wall
        ///// </summary>
        ///// <param name="success"></param>
        //private void PostToWallComplete(bool success)
        //{
        //    this.Dispatcher.BeginInvoke(() =>
        //    {
        //        if (success)
        //        {
        //            MessageBox.Show("Post was successful.");
        //            HideTextEntry();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Unable to post comment to your Facebook wall");
        //        }

        //    });
        //}
        ///// <summary>
        ///// Callback for post to twitter
        ///// </summary>
        ///// <param name="success"></param>
        //private void PostTweetComplete(bool success)
        //{
        //    this.Dispatcher.BeginInvoke(() =>
        //    {
        //        if (success)
        //        {
        //            MessageBox.Show("Tweet was successful.");
        //            HideTextEntry();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Unable to send tweet");
        //        }

        //    });
        //}

        ///// <summary>
        ///// Callback for facebook comment
        ///// </summary>
        ///// <param name="success"></param>
        //private void PostCommentComplete(bool success)
        //{
        //    this.Dispatcher.BeginInvoke(() =>
        //    {
        //        if (success)
        //        {
        //            MessageBox.Show("Post was successful.");
        //        }
        //        else
        //        {
        //            string error;
        //            //if (SecuritySettings.Instance.FacebookUserInfo.DoesntLike)
        //            //{
        //            //    error =
        //            //        "In order to comment on a post you have to 'Facebook Like' this page'. Do you wish to proceed to this Facebook page so you can Like it?";
        //            //}
        //            //else
        //            //{
        //            error =
        //                "Unable to post comment, would you like to proceed to this Facebook page so that you can leave a comment?";
        //            //}
        //            if (
        //                MessageBox.Show(
        //                    error,
        //                    "Visit on Facebook?", MessageBoxButton.OKCancel) ==
        //                MessageBoxResult.OK)
        //            {
        //                Utilities.DisplayInWebBrowser(Activity.SocialActivity.Url);
        //            }
        //        }

        //        HideTextEntry();
        //    });
        //}



        ///// <summary>
        ///// Validate that there are facebook credentials
        ///// </summary>
        ///// <returns></returns>
        //private bool ValidateFacebookCredentials()
        //{
        //    // TODO: Validate Facebook - change to supporting multi-provider posting
        //    /*
        //    if (SecuritySettings.Instance.FacebookUserInfo == null)
        //    {
        //        this.Navigation.Navigate(typeof(FacebookPage));
        //        return false;
        //    }*/
        //    return true;
        //}

        //public int CharactersRemaining
        //{
        //    get { return charactersRemaining; }
        //    set
        //    {
        //        if (CharactersRemaining == value) return;
        //        charactersRemaining = value;
        //        RaisePropertyChanged(() => CharactersRemaining);
        //    }
        //}








        //public Visibility ShareVisibility
        //{
        //    get { return shareVisibility; }
        //    set
        //    {
        //        if (ShareVisibility == value) return;
        //        shareVisibility = value;
        //        RaisePropertyChanged(() => ShareVisibility);
        //    }
        //}

        ///// <summary>
        ///// Display the text entry area
        ///// </summary>
        //private void DisplayTextEntry()
        //{
        //    switch (PostAction)
        //    {
        //        case PostType.Comment:
        //        case PostType.Share:
        //        case PostType.Email:
        //            CharactersRemainingVisibility = Visibility.Collapsed;
        //            break;
        //        case PostType.Tweet:
        //        case PostType.Reply:
        //        case PostType.ReplyAll:
        //        case PostType.Retweet:
        //            CharactersRemainingVisibility = Visibility.Visible;
        //            break;
        //    }
        //    HideAllApplicationBarItems();
        //    ShowApplicationBarItem(ReaderAppBar.ok);
        //    ShowApplicationBarItem(ReaderAppBar.cancel);
        //    ShareVisibility = Visibility.Visible;
        //}


        //public string TextToPost
        //{
        //    get { return textToPost; }
        //    set
        //    {
        //        if (TextToPost == value) return;
        //        textToPost = value;
        //        RaisePropertyChanged(() => TextToPost);

        //        if (CharactersRemainingVisibility == Visibility.Visible)
        //        {
        //            CharactersRemaining = 140 - TextToPost.Length;
        //        }
        //    }
        //}


        public PostPageViewModel(IRepository repository, IDispatcher dispatcher, INavigation navigation)
            : base(repository, dispatcher, navigation)
        {
        }


    }
}
