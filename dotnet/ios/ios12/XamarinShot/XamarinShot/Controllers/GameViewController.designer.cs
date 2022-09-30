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

namespace XamarinShot
{
    [Register ("GameViewController")]
    partial class GameViewController
    {
        [Outlet]
        UIImageView [] teamACatapultImages { get; set; }

        [Outlet]
        UIImageView [] teamBCatapultImages { get; set; }

        [Outlet]
        UIButton [] inSceneButtons { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView activityIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton exitGameButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel instructionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView keyPositionThumbnail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel mappingStateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel networkDelayText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton nextKeyPositionThumbnailButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel notificationLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView overlayView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton previousKeyPositionThumbnailButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton saveAsKeyPositionButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        ARKit.ARSCNView sceneView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton settingsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel thermalStateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel trackingStateLabel { get; set; }

        [Action ("exitGamePressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void exitGamePressed (UIKit.UIButton sender);

        [Action ("handlePan:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handlePan (XamarinShot.Models.GestureRecognizers.ThresholdPanGestureRecognizer sender);

        [Action ("handlePinch:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handlePinch (XamarinShot.Models.GestureRecognizers.ThresholdPinchGestureRecognizer sender);

        [Action ("handleRotation:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handleRotation (XamarinShot.Models.GestureRecognizers.ThresholdRotationGestureRecognizer sender);

        [Action ("handleTap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void handleTap (UIKit.UITapGestureRecognizer sender);

        [Action ("saveAsKeyPositionPressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void saveAsKeyPositionPressed (UIKit.UIButton sender);

        [Action ("showNextKeyPositionThumbnail:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void showNextKeyPositionThumbnail (UIKit.UIButton sender);

        [Action ("showPreviousKeyPositionThumbnail:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void showPreviousKeyPositionThumbnail (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (activityIndicator is not null) {
                activityIndicator.Dispose ();
                activityIndicator = null;
            }

            if (exitGameButton is not null) {
                exitGameButton.Dispose ();
                exitGameButton = null;
            }

            if (instructionLabel is not null) {
                instructionLabel.Dispose ();
                instructionLabel = null;
            }

            if (keyPositionThumbnail is not null) {
                keyPositionThumbnail.Dispose ();
                keyPositionThumbnail = null;
            }

            if (mappingStateLabel is not null) {
                mappingStateLabel.Dispose ();
                mappingStateLabel = null;
            }

            if (networkDelayText is not null) {
                networkDelayText.Dispose ();
                networkDelayText = null;
            }

            if (nextKeyPositionThumbnailButton is not null) {
                nextKeyPositionThumbnailButton.Dispose ();
                nextKeyPositionThumbnailButton = null;
            }

            if (notificationLabel is not null) {
                notificationLabel.Dispose ();
                notificationLabel = null;
            }

            if (overlayView is not null) {
                overlayView.Dispose ();
                overlayView = null;
            }

            if (previousKeyPositionThumbnailButton is not null) {
                previousKeyPositionThumbnailButton.Dispose ();
                previousKeyPositionThumbnailButton = null;
            }

            if (saveAsKeyPositionButton is not null) {
                saveAsKeyPositionButton.Dispose ();
                saveAsKeyPositionButton = null;
            }

            if (sceneView is not null) {
                sceneView.Dispose ();
                sceneView = null;
            }

            if (settingsButton is not null) {
                settingsButton.Dispose ();
                settingsButton = null;
            }

            if (thermalStateLabel is not null) {
                thermalStateLabel.Dispose ();
                thermalStateLabel = null;
            }

            if (trackingStateLabel is not null) {
                trackingStateLabel.Dispose ();
                trackingStateLabel = null;
            }
        }
    }
}