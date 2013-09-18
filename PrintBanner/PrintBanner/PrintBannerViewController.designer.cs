// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace PrintBanner
{
	[Register ("PrintBannerViewController")]
	partial class PrintBannerViewController
	{
		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UISegmentedControl colorSelection { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UISegmentedControl fontSelection { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIButton printButton { get; set; }

		[Outlet]
		[GeneratedCodeAttribute ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UITextField textField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (textField != null) {
				textField.Dispose ();
				textField = null;
			}

			if (fontSelection != null) {
				fontSelection.Dispose ();
				fontSelection = null;
			}

			if (colorSelection != null) {
				colorSelection.Dispose ();
				colorSelection = null;
			}

			if (printButton != null) {
				printButton.Dispose ();
				printButton = null;
			}
		}
	}
}
