using Foundation;
using System;
using UIKit;

namespace UICatalog
{
    public partial class CustomSearchBarViewController : UIViewController, IUISearchBarDelegate
    {
        public CustomSearchBarViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ConfigureSearchBar();
        }

        private void ConfigureSearchBar()
        {
            searchBar.ShowsCancelButton = true;
            searchBar.ShowsBookmarkButton = true;

            searchBar.TintColor = UIColor.Purple;
            searchBar.BackgroundImage = UIImage.FromBundle("search_bar_background");

            // Set the bookmark image for both normal and highlighted states.
            var bookmarkImage = UIImage.FromBundle("bookmark_icon");
            searchBar.SetImageforSearchBarIcon(bookmarkImage, UISearchBarIcon.Bookmark, UIControlState.Normal);

            var bookmarkHighlightedImage = UIImage.FromBundle("bookmark_icon_highlighted");
            searchBar.SetImageforSearchBarIcon(bookmarkHighlightedImage, UISearchBarIcon.Bookmark, UIControlState.Highlighted);
        }

        #region IUISearchBarDelegate

        [Export("searchBarSearchButtonClicked:")]
        public void SearchButtonClicked(UISearchBar searchBar)
        {
            Console.WriteLine("The custom search bar keyboard search button was tapped.");
            searchBar.ResignFirstResponder();
        }

        [Export("searchBarCancelButtonClicked:")]
        public void CancelButtonClicked(UISearchBar searchBar)
        {
            Console.WriteLine("The custom search bar cancel button was tapped.");
            searchBar.ResignFirstResponder();
        }

        [Export("searchBarBookmarkButtonClicked:")]
        public void BookmarkButtonClicked(UISearchBar searchBar)
        {
            Console.WriteLine("The custom bookmark button inside the search bar was tapped.");
        }

        #endregion
    }
}