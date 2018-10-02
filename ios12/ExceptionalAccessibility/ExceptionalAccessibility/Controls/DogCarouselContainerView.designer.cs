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
    [Register ("DogCarouselContainerView")]
    partial class DogCarouselContainerView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UICollectionView dogCollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UIButton galleryButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (dogCollectionView != null) {
                dogCollectionView.Dispose ();
                dogCollectionView = null;
            }

            if (galleryButton != null) {
                galleryButton.Dispose ();
                galleryButton = null;
            }
        }
    }
}