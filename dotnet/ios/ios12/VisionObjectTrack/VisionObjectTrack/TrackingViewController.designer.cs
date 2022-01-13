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

namespace VisionObjectTrack
{
    [Register ("TrackingViewController")]
    partial class TrackingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton clearRectsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl entitySelector { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel frameCounterLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView frameCounterLabelBackplate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl modeSelector { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView settingsView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem startStopButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        VisionObjectTrack.TrackingImageView trackingView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint trackingViewTopConstraint { get; set; }

        [Action ("handleClearRectsButton:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handleClearRectsButton (UIKit.UIButton sender);

        [Action ("handleEntitySelection:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handleEntitySelection (UIKit.UISegmentedControl sender);

        [Action ("handleModeSelection:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handleModeSelection (UIKit.UISegmentedControl sender);

        [Action ("handlePan:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handlePan (UIKit.UIPanGestureRecognizer sender);

        [Action ("handleStartStopButton:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handleStartStopButton (UIKit.UIBarButtonItem sender);

        [Action ("handleTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handleTap (UIKit.UITapGestureRecognizer sender);

        void ReleaseDesignerOutlets ()
        {
            if (clearRectsButton != null) {
                clearRectsButton.Dispose ();
                clearRectsButton = null;
            }

            if (entitySelector != null) {
                entitySelector.Dispose ();
                entitySelector = null;
            }

            if (frameCounterLabel != null) {
                frameCounterLabel.Dispose ();
                frameCounterLabel = null;
            }

            if (frameCounterLabelBackplate != null) {
                frameCounterLabelBackplate.Dispose ();
                frameCounterLabelBackplate = null;
            }

            if (modeSelector != null) {
                modeSelector.Dispose ();
                modeSelector = null;
            }

            if (settingsView != null) {
                settingsView.Dispose ();
                settingsView = null;
            }

            if (startStopButton != null) {
                startStopButton.Dispose ();
                startStopButton = null;
            }

            if (trackingView != null) {
                trackingView.Dispose ();
                trackingView = null;
            }

            if (trackingViewTopConstraint != null) {
                trackingViewTopConstraint.Dispose ();
                trackingViewTopConstraint = null;
            }
        }
    }
}