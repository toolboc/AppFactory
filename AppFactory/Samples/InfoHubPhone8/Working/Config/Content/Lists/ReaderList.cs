using System.Collections.Generic;
using System.Windows.Markup;
using InfoHubPhone8.Config.Content.Lists.Filters;

namespace InfoHubPhone8.Config.Content.Lists
{
    [ContentProperty("Filters")]
    public class ReaderList : IReaderList
    {
        private string title;
        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(title))
                {
                    return ListName;
                }
                return title;
            }

            set { title = value; }
        }



        private string shortTitle;
        public string ShortTitle
        {
            get
            {
                if (string.IsNullOrEmpty(shortTitle))
                {
                    return Title;
                }
                return shortTitle;
            }

            set { shortTitle = value; }
        }


        public string ListName { get; set; }

        public List<IListFilter> Filters { get; private set; }

        public ReaderList()
        {
            Filters = new List<IListFilter>();
        }
    }
}