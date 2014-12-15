// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace StateRestoration
{
	[Register ("FilterViewController")]
	partial class FilterViewController
	{
		[Outlet]
		MonoTouch.UIKit.UISwitch ActiveSwitch { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider Slider { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ActiveSwitch != null) {
				ActiveSwitch.Dispose ();
				ActiveSwitch = null;
			}

			if (Slider != null) {
				Slider.Dispose ();
				Slider = null;
			}
		}
	}
}
