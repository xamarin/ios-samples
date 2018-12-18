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
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView colorView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPageControl pageControl { get; set; }

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