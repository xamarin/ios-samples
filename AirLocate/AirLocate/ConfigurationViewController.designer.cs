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
	[Register ("ConfigurationViewController")]
	partial class ConfigurationViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch enabledSwitch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField majorTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField measuredPowerTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField minorTextField { get; set; }

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
			if (measuredPowerTextField != null) {
				measuredPowerTextField.Dispose ();
				measuredPowerTextField = null;
			}
			if (minorTextField != null) {
				minorTextField.Dispose ();
				minorTextField = null;
			}
			if (uuidTextField != null) {
				uuidTextField.Dispose ();
				uuidTextField = null;
			}
		}
	}
}
