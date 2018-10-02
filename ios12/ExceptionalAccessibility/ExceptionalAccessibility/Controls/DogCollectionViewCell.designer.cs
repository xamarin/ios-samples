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
    [Register ("DogCollectionViewCell")]
    partial class DogCollectionViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UIImageView dogImageView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (dogImageView != null) {
                dogImageView.Dispose ();
                dogImageView = null;
            }
        }
    }
}