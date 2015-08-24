// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WK2X
{
	[Register ("InterfaceController")]
	partial class InterfaceController
	{
		[Outlet]
		WatchKit.WKInterfaceButton myButton { get; set; }

		[Outlet]
		WatchKit.WKInterfaceLabel myLabel { get; set; }

		[Action ("OnButtonPress")]
		partial void OnButtonPress ();
		
		void ReleaseDesignerOutlets ()
		{
			if (myButton != null) {
				myButton.Dispose ();
				myButton = null;
			}

			if (myLabel != null) {
				myLabel.Dispose ();
				myLabel = null;
			}
		}
	}
}
