// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace UICatalog
{
	[Register ("SwitchesViewController")]
	partial class SwitchesViewController
	{
		[Outlet]
		UIKit.UISwitch defaultSwitch { get; set; }

		[Outlet]
		UIKit.UISwitch tintedSwitch { get; set; }

		[Action ("DefaultSwittcherValueChanged:")]
		partial void DefaultSwittcherValueChanged (Foundation.NSObject sender);

		[Action ("DefaultValueChanged:")]
		partial void DefaultValueChanged (Foundation.NSObject sender);

		[Action ("TintedValueChanged:")]
		partial void TintedValueChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (defaultSwitch != null) {
				defaultSwitch.Dispose ();
				defaultSwitch = null;
			}

			if (tintedSwitch != null) {
				tintedSwitch.Dispose ();
				tintedSwitch = null;
			}
		}
	}
}
