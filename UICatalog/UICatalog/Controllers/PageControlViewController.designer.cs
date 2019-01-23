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
    [Register ("PageControlViewController")]
    partial class PageControlViewController
    {
        [Outlet]
        UIKit.UIView colorView { get; set; }


        [Outlet]
        UIKit.UIPageControl pageControl { get; set; }


        [Action ("PageControllerValueChanged:")]
        partial void PageControllerValueChanged (Foundation.NSObject sender);


        [Action ("PageControlValueChanged:")]
        partial void PageControlValueChanged (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (colorView != null) {
                colorView.Dispose ();
                colorView = null;
            }

            if (pageControl != null) {
                pageControl.Dispose ();
                pageControl = null;
            }
        }
    }
}