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
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider customSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider defaultSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider tintedSlider { get; set; }

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