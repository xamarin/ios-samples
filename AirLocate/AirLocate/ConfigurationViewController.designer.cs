// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace AirLocate
{
	[Register ("ConfigurationViewController")]
	partial class ConfigurationViewController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UISwitch enabledSwitch { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UITextField majorTextField { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UITextField measuredPowerTextField { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UITextField minorTextField { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UITextField uuidTextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (majorTextField != null) {
				majorTextField.Dispose ();
				majorTextField = null;
			}

			if (minorTextField != null) {
				minorTextField.Dispose ();
				minorTextField = null;
			}

			if (measuredPowerTextField != null) {
				measuredPowerTextField.Dispose ();
				measuredPowerTextField = null;
			}

			if (enabledSwitch != null) {
				enabledSwitch.Dispose ();
				enabledSwitch = null;
			}

			if (uuidTextField != null) {
				uuidTextField.Dispose ();
				uuidTextField = null;
			}
		}
	}
}
