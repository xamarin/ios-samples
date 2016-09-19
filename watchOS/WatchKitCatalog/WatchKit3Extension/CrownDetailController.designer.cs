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
	[Register ("CrownDetailController")]
	partial class CrownDetailController
	{
		[Outlet]
		WatchKit.WKInterfacePicker pickerView { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel stateLabel { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel velocityLabel { get; set; }

		[Action ("FocusCrown:")]
		partial void FocusCrown (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (pickerView != null) {
				pickerView.Dispose ();
				pickerView = null;
			}

			if (stateLabel != null) {
				stateLabel.Dispose ();
				stateLabel = null;
			}

			if (velocityLabel != null) {
				velocityLabel.Dispose ();
				velocityLabel = null;
			}
		}
	}
}
