using Foundation;
using System;
using UIKit;

namespace UICatalog {
	public partial class TintedToolbarViewController : UIViewController {
		public TintedToolbarViewController (IntPtr handle) : base (handle) { }

		partial void ActionTapped (NSObject sender)
		{
			Console.WriteLine ("'Action' bar button item was clicked");
		}

		partial void RefreshTapped (NSObject sender)
		{
			Console.WriteLine ("'Refresh' bar button item was clicked");
		}
	}
}
