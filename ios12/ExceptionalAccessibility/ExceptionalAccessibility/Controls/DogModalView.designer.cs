// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ExceptionalAccessibility
{
    [Register ("DogModalView")]
    partial class DogModalView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UIButton closeButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UIImageView firstImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UIImageView secondImageView { get; set; }

        [Action ("closeButtonTapped:")]
        partial void closeButtonTapped (UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (closeButton != null) {
                closeButton.Dispose ();
                closeButton = null;
            }

            if (firstImageView != null) {
                firstImageView.Dispose ();
                firstImageView = null;
            }

            if (secondImageView != null) {
                secondImageView.Dispose ();
                secondImageView = null;
            }
        }
    }
}