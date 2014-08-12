using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class CustomSearchBarViewController : UIViewController
	{
		[Outlet]
		private UISearchBar SearchBar { get; set; }

		public CustomSearchBarViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureSearchBar ();
		}

		private void ConfigureSearchBar()
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

		[Export("searchBarSearchButtonClicked:")]
		private void OnSearchBarSearchButtonClicked(UISearchBar searchBar)
		{
			Console.WriteLine ("The custom search bar keyboard search button was tapped: {0}.", searchBar);
			searchBar.ResignFirstResponder ();
		}

		[Export("searchBarCancelButtonClicked:")]
		private void OnSearchBarCancelButtonClicked(UISearchBar searchBar)
		{
			Console.WriteLine ("The custom search bar cancel button was tapped.");
			searchBar.ResignFirstResponder ();
		}

		[Export("searchBarBookmarkButtonClicked:")]
		private void OnSearchBarBookmarkButtonClicked(UISearchBar searchBar)
		{
			Console.WriteLine ("The custom bookmark button inside the search bar was tapped.");
		}

		#endregion
	}
}
