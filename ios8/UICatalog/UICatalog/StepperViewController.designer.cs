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
	[Register ("StepperViewController")]
	partial class StepperViewController
	{
		[Outlet]
		UIKit.UILabel customLabel { get; set; }

		[Outlet]
		UIKit.UIStepper customStepper { get; set; }

		[Outlet]
		UIKit.UILabel defaultLabel { get; set; }

		[Outlet]
		UIKit.UIStepper defaultStepper { get; set; }

		[Outlet]
		UIKit.UILabel tintedLabel { get; set; }

		[Outlet]
		UIKit.UIStepper tintedStepper { get; set; }

		[Action ("CustomValueChanged:")]
		partial void CustomValueChanged (Foundation.NSObject sender);

		[Action ("DefaultValueChanged:")]
		partial void DefaultValueChanged (Foundation.NSObject sender);

		[Action ("TintedValueChanged:")]
		partial void TintedValueChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (customStepper != null) {
				customStepper.Dispose ();
				customStepper = null;
			}

			if (defaultStepper != null) {
				defaultStepper.Dispose ();
				defaultStepper = null;
			}

			if (tintedStepper != null) {
				tintedStepper.Dispose ();
				tintedStepper = null;
			}

			if (defaultLabel != null) {
				defaultLabel.Dispose ();
				defaultLabel = null;
			}

			if (tintedLabel != null) {
				tintedLabel.Dispose ();
				tintedLabel = null;
			}

			if (customLabel != null) {
				customLabel.Dispose ();
				customLabel = null;
			}
		}
	}
}
