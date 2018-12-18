using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("CustomSearchBarViewController")]
	public class CustomSearchBarViewController : UIViewController
	{
		[Outlet]
		UISearchBar SearchBar { get; set; }

		public CustomSearchBarViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureSearchBar ();
		}

		void ConfigureSearchBar ()
		{
			SearchBar.ShowsCancelButton = true;
			SearchBar.ShowsBookmarkButton = true;

			SearchBar.TintColor = ApplicationColors.Purple;
			SearchBar.BackgroundImage = UIImage.FromBundle ("search_bar_background");

			// Set the bookmark image for both normal and highlighted states.
			var bookmarkImage = UIImage.FromBundle ("bookmark_icon");
			SearchBar.SetImageforSearchBarIcon (bookmarkImage, UISearchBarIcon.Bookmark, UIControlState.Normal);

			var bookmarkHighlightedImage = UIImage.FromBundle ("bookmark_icon_highlighted");
			SearchBar.SetImageforSearchBarIcon (bookmarkHighlightedImage, UISearchBarIcon.Bookmark, UIControlState.Highlighted);
		}

		#region UISearchBarDelegate

		[Export ("searchBarSearchButtonClicked:")]
		void OnSearchBarSearchButtonClicked (UISearchBar searchBar)
		{
			Console.WriteLine ("The custom search bar keyboard search button was tapped: {0}.", searchBar);
			searchBar.ResignFirstResponder ();
		}

		[Export ("searchBarCancelButtonClicked:")]
		void OnSearchBarCancelButtonClicked (UISearchBar searchBar)
		{
			Console.WriteLine ("The custom search bar cancel button was tapped.");
			searchBar.ResignFirstResponder ();
		}

		[Export ("searchBarBookmarkButtonClicked:")]
		void OnSearchBarBookmarkButtonClicked (UISearchBar searchBar)
		{
			Console.WriteLine ("The custom bookmark button inside the search bar was tapped.");
		}

		#endregion
	}
}
