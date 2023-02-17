using Foundation;
using System;
using UIKit;

namespace UICatalog {
	public partial class DefaultToolbarViewController : UIViewController {
		public DefaultToolbarViewController (IntPtr handle) : base (handle) { }

		partial void ActionTapped (NSObject sender)
		{
			Console.WriteLine ("'Action' bar button item was clicked");
		}

		partial void TrashTapped (NSObject sender)
		{
			Console.WriteLine ("'Trash' bar button item was clicked");
		}
	}
}
