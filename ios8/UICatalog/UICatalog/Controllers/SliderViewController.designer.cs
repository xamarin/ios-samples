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
	[Register ("SliderViewController")]
	partial class SliderViewController
	{
		[Outlet]
		UIKit.UISlider customSlider { get; set; }

		[Outlet]
		UIKit.UISlider defaultSlider { get; set; }

		[Outlet]
		UIKit.UISlider tintedSlider { get; set; }

		[Action ("CustomSliderValueChanged:")]
		partial void CustomSliderValueChanged (Foundation.NSObject sender);

		[Action ("CustomSliderValurChanged:")]
		partial void CustomSliderValurChanged (Foundation.NSObject sender);

		[Action ("DefaultSliderValueChanged:")]
		partial void DefaultSliderValueChanged (Foundation.NSObject sender);

		[Action ("TintedSliderValueChanged:")]
		partial void TintedSliderValueChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (defaultSlider != null) {
				defaultSlider.Dispose ();
				defaultSlider = null;
			}

			if (tintedSlider != null) {
				tintedSlider.Dispose ();
				tintedSlider = null;
			}

			if (customSlider != null) {
				customSlider.Dispose ();
				customSlider = null;
			}
		}
	}
}
