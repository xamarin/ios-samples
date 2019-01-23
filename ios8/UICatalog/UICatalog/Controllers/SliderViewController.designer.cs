// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
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
            if (customSlider != null) {
                customSlider.Dispose ();
                customSlider = null;
            }

            if (defaultSlider != null) {
                defaultSlider.Dispose ();
                defaultSlider = null;
            }

            if (tintedSlider != null) {
                tintedSlider.Dispose ();
                tintedSlider = null;
            }
        }
    }
}