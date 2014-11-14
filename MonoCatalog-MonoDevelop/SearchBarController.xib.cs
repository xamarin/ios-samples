//
// C# port of the searchbar sample
//
using Foundation;
using UIKit;
using CoreGraphics;

namespace MonoCatalog {
	
	public partial class SearchBarController : UIViewController {
		UISearchBar bar;
	
		class SearchDelegate : UISearchBarDelegate {
			public override void SearchButtonClicked (UISearchBar bar)
			{
				bar.ResignFirstResponder ();
			}
	
			public override void CancelButtonClicked (UISearchBar bar)
			{
				bar.ResignFirstResponder ();
			}
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Search Bar";
			NavigationController.NavigationBar.Translucent = false;
			View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
				var f = new CGRect (0f, 64f, View.Bounds.Width, 44f);
			bar = new UISearchBar (f){
				Delegate = new SearchDelegate (),
				ShowsCancelButton = true,
			};
			View.AddSubview (bar);
		}
		
	}
}
