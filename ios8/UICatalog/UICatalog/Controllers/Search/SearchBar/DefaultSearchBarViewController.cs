using Foundation;
using System;
using UIKit;

namespace UICatalog
{
    public partial class DefaultSearchBarViewController : UIViewController, IUISearchBarDelegate
    {
        public DefaultSearchBarViewController(IntPtr handle) : base(handle) { }

        [Export("searchBar:selectedScopeButtonIndexDidChange:")]
        public void SelectedScopeButtonIndexChanged(UISearchBar searchBar, nint selectedScope)
        {
            Console.WriteLine($"The default search selected scope button index changed to {selectedScope}.");
        }

        [Export("searchBarSearchButtonClicked:")]
        public void SearchButtonClicked(UISearchBar searchBar)
        {
            Console.WriteLine("The default search bar keyboard search button was tapped");
            searchBar.ResignFirstResponder();
        }

        [Export("searchBarCancelButtonClicked:")]
        public void CancelButtonClicked(UISearchBar searchBar)
        {
            Console.WriteLine("The default search bar cancel button was tapped.");
            searchBar.ResignFirstResponder();
        }
    }
}