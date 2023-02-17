using Foundation;
using System;
using UIKit;

namespace largetitles {
	public partial class NavigationController : UINavigationController {
		public NavigationController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationBar.PrefersLargeTitles = true;
		}
	}
}
