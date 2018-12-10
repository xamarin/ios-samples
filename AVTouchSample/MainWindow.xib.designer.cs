// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace avTouch
{
    [Register ("avTouchAppDelegate")]
    partial class avTouchAppDelegate
    {
        [Outlet]
        avTouch.avTouchViewController viewController { get; set; }


        [Outlet]
        UIKit.UIWindow window { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (window != null) {
                window.Dispose ();
                window = null;
            }
        }
    }
}