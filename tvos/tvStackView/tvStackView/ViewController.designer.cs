// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MySingleView
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIView Player1 { get; set; }

		[Outlet]
		UIKit.UIView Player2 { get; set; }

		[Outlet]
		UIKit.UIView Player3 { get; set; }

		[Outlet]
		UIKit.UIView Player4 { get; set; }

		[Outlet]
		UIKit.UISegmentedControl PlayerCount { get; set; }

		[Action ("PlayerCountChanged:")]
		partial void PlayerCountChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Player1 != null) {
				Player1.Dispose ();
				Player1 = null;
			}

			if (Player2 != null) {
				Player2.Dispose ();
				Player2 = null;
			}

			if (Player3 != null) {
				Player3.Dispose ();
				Player3 = null;
			}

			if (Player4 != null) {
				Player4.Dispose ();
				Player4 = null;
			}

			if (PlayerCount != null) {
				PlayerCount.Dispose ();
				PlayerCount = null;
			}
		}
	}
}
