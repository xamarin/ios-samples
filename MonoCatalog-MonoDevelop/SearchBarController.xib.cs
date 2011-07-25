//
// C# port of the searchbar sample
//
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

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
			View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
			var f = new RectangleF (0f, 0f, View.Bounds.Width, 44f);
			bar = new UISearchBar (f){
				Delegate = new SearchDelegate (),
				ShowsCancelButton = true,
			};
			View.AddSubview (bar);
		}
		
	}
}
