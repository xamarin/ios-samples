// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WatchkitExtension
{
	[Register ("SwitchDetailController")]
	partial class SwitchDetailController
	{
		[Outlet]
		WatchKit.WKInterfaceSwitch coloredSwitch { get; set; }

		[Outlet]
		WatchKit.WKInterfaceSwitch offSwitch { get; set; }

		[Action ("switchAction:")]
		partial void SwitchAction (System.Boolean value);
		
		void ReleaseDesignerOutlets ()
		{
			if (offSwitch != null) {
				offSwitch.Dispose ();
				offSwitch = null;
			}

			if (coloredSwitch != null) {
				coloredSwitch.Dispose ();
				coloredSwitch = null;
			}
		}
	}
}
