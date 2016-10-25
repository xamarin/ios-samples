// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Watchkit2Extension
{
	[Register ("GestureDetailController")]
	partial class GestureDetailController
	{
		[Outlet]
		WatchKit.WKInterfaceGroup longPresGroup { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel longPressLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceGroup panGroup { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel panLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceGroup swipeGroup { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel swipeLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceGroup tapGroup { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel tapLabel { get; set; }

		[Action ("LongPressRecognized:")]
		partial void LongPressRecognized (Foundation.NSObject sender);

		[Action ("PanRecognized:")]
		partial void PanRecognized (Foundation.NSObject sender);

		[Action ("SwipeRecognized:")]
		partial void SwipeRecognized (Foundation.NSObject sender);

		[Action ("TapRecognized:")]
		partial void TapRecognized (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (longPressLabel != null) {
				longPressLabel.Dispose ();
				longPressLabel = null;
			}

			if (longPresGroup != null) {
				longPresGroup.Dispose ();
				longPresGroup = null;
			}

			if (panLabel != null) {
				panLabel.Dispose ();
				panLabel = null;
			}

			if (swipeGroup != null) {
				swipeGroup.Dispose ();
				swipeGroup = null;
			}

			if (swipeLabel != null) {
				swipeLabel.Dispose ();
				swipeLabel = null;
			}

			if (tapGroup != null) {
				tapGroup.Dispose ();
				tapGroup = null;
			}

			if (tapLabel != null) {
				tapLabel.Dispose ();
				tapLabel = null;
			}

			if (panGroup != null) {
				panGroup.Dispose ();
				panGroup = null;
			}
		}
	}
}
