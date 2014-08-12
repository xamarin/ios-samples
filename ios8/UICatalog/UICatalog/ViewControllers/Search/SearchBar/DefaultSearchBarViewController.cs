using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class DefaultSearchBarViewController : UIViewController
	{
		[Outlet]
		private UISearchBar SearchBar { get; set; }

		public DefaultSearchBarViewController (IntPtr handle)
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
			SearchBar.ShowsScopeBar = true;

			SearchBar.ScopeButtonTitles = new string[] {
				"Scope One".Localize (),
				"Scope Two".Localize ()
			};
		}

		#region UISearchBarDelegate

		[Export("searchBar:selectedScopeButtonIndexDidChange:")]
		private void OnSelectedScopeButtonIndexChange(UISearchBar searchBar, int selectedScope)
		{
			Console.WriteLine ("The default search selected scope button index changed to {0}.", selectedScope);
		}

		[Export("searchBarSearchButtonClicked:")]
		private void OnSearchBarSearchButtonClicked(UISearchBar searchBar)
		{
			Console.WriteLine ("The default search bar keyboard search button was tapped: {0}", searchBar);
			searchBar.ResignFirstResponder ();
		}

		[Export("searchBarCancelButtonClicked:")]
		private void searchBarCancelButtonClicked(UISearchBar searchBar)
		{
			Console.WriteLine ("The default search bar cancel button was tapped.");
			searchBar.ResignFirstResponder();
		}

		#endregion
	}
}
