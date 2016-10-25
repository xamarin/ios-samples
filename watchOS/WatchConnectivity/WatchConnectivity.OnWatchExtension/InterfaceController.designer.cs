// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WatchConnectivity.OnWatchExtension
{
	[Register ("InterfaceController")]
	partial class InterfaceController
	{
		[Outlet]
		WatchKit.WKInterfaceButton bottomLeft { get; set; }

		[Outlet]
		WatchKit.WKInterfaceButton bottomRight { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel label { get; set; }

		[Outlet]
		WatchKit.WKInterfaceButton topLeft { get; set; }

		[Outlet]
		WatchKit.WKInterfaceButton topRight { get; set; }

		[Action ("BottomLeftPressed")]
		partial void BottomLeftPressed ();

		[Action ("BottomRightPressed")]
		partial void BottomRightPressed ();

		[Action ("TopLeftPressed")]
		partial void TopLeftPressed ();

		[Action ("TopRightPressed")]
		partial void TopRightPressed ();
		
		void ReleaseDesignerOutlets ()
		{
			if (bottomLeft != null) {
				bottomLeft.Dispose ();
				bottomLeft = null;
			}

			if (bottomRight != null) {
				bottomRight.Dispose ();
				bottomRight = null;
			}

			if (label != null) {
				label.Dispose ();
				label = null;
			}

			if (topLeft != null) {
				topLeft.Dispose ();
				topLeft = null;
			}

			if (topRight != null) {
				topRight.Dispose ();
				topRight = null;
			}
		}
	}
}
