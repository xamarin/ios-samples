// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace AirLocate
{
	[Register ("MonitoringViewController")]
	partial class MonitoringViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch enabledSwitch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField majorTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField minorTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch notifyOnDisplaySwitch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch notifyOnEntrySwitch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch notifyOnExitSwitch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField uuidTextField { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (enabledSwitch != null) {
				enabledSwitch.Dispose ();
				enabledSwitch = null;
			}
			if (majorTextField != null) {
				majorTextField.Dispose ();
				majorTextField = null;
			}
			if (minorTextField != null) {
				minorTextField.Dispose ();
				minorTextField = null;
			}
			if (notifyOnDisplaySwitch != null) {
				notifyOnDisplaySwitch.Dispose ();
				notifyOnDisplaySwitch = null;
			}
			if (notifyOnEntrySwitch != null) {
				notifyOnEntrySwitch.Dispose ();
				notifyOnEntrySwitch = null;
			}
			if (notifyOnExitSwitch != null) {
				notifyOnExitSwitch.Dispose ();
				notifyOnExitSwitch = null;
			}
			if (uuidTextField != null) {
				uuidTextField.Dispose ();
				uuidTextField = null;
			}
		}
	}
}
