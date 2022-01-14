// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ExceptionalAccessibility
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton callButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        ExceptionalAccessibility.DogCarouselContainerView carouselContainerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UICollectionView dogCollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        ExceptionalAccessibility.DogStatsView dogStatsView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton galleryButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton locationButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView shelterInfoView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel shelterNameLabel { get; set; }

        [Action ("galleryButtonPressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void galleryButtonPressed (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (callButton != null) {
                callButton.Dispose ();
                callButton = null;
            }

            if (carouselContainerView != null) {
                carouselContainerView.Dispose ();
                carouselContainerView = null;
            }

            if (dogCollectionView != null) {
                dogCollectionView.Dispose ();
                dogCollectionView = null;
            }

            if (dogStatsView != null) {
                dogStatsView.Dispose ();
                dogStatsView = null;
            }

            if (galleryButton != null) {
                galleryButton.Dispose ();
                galleryButton = null;
            }

            if (locationButton != null) {
                locationButton.Dispose ();
                locationButton = null;
            }

            if (shelterInfoView != null) {
                shelterInfoView.Dispose ();
                shelterInfoView = null;
            }

            if (shelterNameLabel != null) {
                shelterNameLabel.Dispose ();
                shelterNameLabel = null;
            }
        }
    }
}