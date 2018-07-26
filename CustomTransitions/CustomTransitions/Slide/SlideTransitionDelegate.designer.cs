// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CustomTransitions
{
    [Register ("SlideTransitionDelegate")]
    partial class SlideTransitionDelegate
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITabBarController tabBarController { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tabBarController != null) {
                tabBarController.Dispose ();
                tabBarController = null;
            }
        }
    }
}