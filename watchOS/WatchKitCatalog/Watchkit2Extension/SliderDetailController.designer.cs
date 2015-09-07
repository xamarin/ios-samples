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
	[Register ("SliderDetailController")]
	partial class SliderDetailController
	{
		[Outlet]
		WatchKit.WKInterfaceSlider coloredSlider { get; set; }

		[Action ("sliderAction:")]
		partial void SliderAction (System.Single value);
		
		void ReleaseDesignerOutlets ()
		{
			if (coloredSlider != null) {
				coloredSlider.Dispose ();
				coloredSlider = null;
			}
		}
	}
}
