// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace AVCamBarcode
{
    [Register ("CameraViewController")]
    partial class CameraViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CameraButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CameraUnavailableLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MetadataObjectTypesButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        AVCamBarcode.PreviewView PreviewView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SessionPresetsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider ZoomSlider { get; set; }

        [Action ("ChangeCamera:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChangeCamera (UIKit.UIButton sender);

        [Action ("SelectMetadataObjectTypes:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SelectMetadataObjectTypes (UIKit.UIButton sender);

        [Action ("SelectSessionPreset:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SelectSessionPreset (UIKit.UIButton sender);

        [Action ("ZoomCamera:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ZoomCamera (UIKit.UISlider sender);

        void ReleaseDesignerOutlets ()
        {
            if (CameraButton != null) {
                CameraButton.Dispose ();
                CameraButton = null;
            }

            if (CameraUnavailableLabel != null) {
                CameraUnavailableLabel.Dispose ();
                CameraUnavailableLabel = null;
            }

            if (MetadataObjectTypesButton != null) {
                MetadataObjectTypesButton.Dispose ();
                MetadataObjectTypesButton = null;
            }

            if (PreviewView != null) {
                PreviewView.Dispose ();
                PreviewView = null;
            }

            if (SessionPresetsButton != null) {
                SessionPresetsButton.Dispose ();
                SessionPresetsButton = null;
            }

            if (ZoomSlider != null) {
                ZoomSlider.Dispose ();
                ZoomSlider = null;
            }
        }
    }
}