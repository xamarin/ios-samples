using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("DefaultSearchBarViewController")]
	public class DefaultSearchBarViewController : UIViewController
	{
		[Outlet]
		UISearchBar SearchBar { get; set; }

		public DefaultSearchBarViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureSearchBar ();
		}

		void ConfigureSearchBar()
		{
			SearchBar.ShowsCancelButton = true;
			SearchBar.ShowsScopeBar = true;

			SearchBar.ScopeButtonTitles = new string[] {
				"Scope One".Localize (),
				"Scope Two".Localize ()
			};
		}

		#region UISearchBarDelegate

		[Export("searchBar:selectedScopeButtonIndexDidChange:")]
		void OnSelectedScopeButtonIndexChange(UISearchBar searchBar, int selectedScope)
		{
			Console.WriteLine ("The default search selected scope button index changed to {0}.", selectedScope);
		}

		[Export("searchBarSearchButtonClicked:")]
		void OnSearchBarSearchButtonClicked(UISearchBar searchBar)
		{
			Console.WriteLine ("The default search bar keyboard search button was tapped: {0}", searchBar);
			searchBar.ResignFirstResponder ();
		}

		[Export("searchBarCancelButtonClicked:")]
		void searchBarCancelButtonClicked(UISearchBar searchBar)
		{
			Console.WriteLine ("The default search bar cancel button was tapped.");
			searchBar.ResignFirstResponder();
		}

		#endregion
	}
}
