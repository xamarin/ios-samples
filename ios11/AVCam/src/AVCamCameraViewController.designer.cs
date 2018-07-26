// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AVCam
{
	[Register ("AVCamCameraViewController")]
	partial class AVCamCameraViewController
	{
		[Outlet]
		UIKit.UIButton CameraButton { get; set; }

		[Outlet]
		UIKit.UILabel CameraUnavailableLabel { get; set; }

		[Outlet]
		UIKit.UISegmentedControl CaptureModeControl { get; set; }

		[Outlet]
		UIKit.UILabel CapturingLivePhotoLabel { get; set; }

		[Outlet]
		UIKit.UIButton DepthDataDeliveryButton { get; set; }

		[Outlet]
		UIKit.UIButton LivePhotoModeButton { get; set; }

		[Outlet]
		UIKit.UIButton PhotoButton { get; set; }

		[Outlet]
		AVCam.AVCamPreviewView PreviewView { get; set; }

		[Outlet]
		UIKit.UIButton RecordButton { get; set; }

		[Outlet]
		UIKit.UIButton ResumeButton { get; set; }

		[Action ("CapturePhoto:")]
		partial void CapturePhoto (Foundation.NSObject sender);

		[Action ("ChangeCamera:")]
		partial void ChangeCamera (Foundation.NSObject sender);

		[Action ("FocusAndExposeTap:")]
		partial void FocusAndExposeTap (UIKit.UIGestureRecognizer gestureRecognizer);

		[Action ("ResumeInterruptedSession:")]
		partial void ResumeInterruptedSession (Foundation.NSObject sender);

		[Action ("ToggleCaptureMode:")]
		partial void ToggleCaptureMode (UIKit.UISegmentedControl captureModeControl);

		[Action ("ToggleDepthDataDeliveryMode:")]
		partial void ToggleDepthDataDeliveryMode (UIKit.UIButton depthDataDeliveryButton);

		[Action ("ToggleLivePhotoMode:")]
		partial void ToggleLivePhotoMode (UIKit.UIButton livePhotoModeButton);

		[Action ("ToggleMovieRecording:")]
		partial void ToggleMovieRecording (Foundation.NSObject sender);
		
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

			if (CaptureModeControl != null) {
				CaptureModeControl.Dispose ();
				CaptureModeControl = null;
			}

			if (CapturingLivePhotoLabel != null) {
				CapturingLivePhotoLabel.Dispose ();
				CapturingLivePhotoLabel = null;
			}

			if (DepthDataDeliveryButton != null) {
				DepthDataDeliveryButton.Dispose ();
				DepthDataDeliveryButton = null;
			}

			if (LivePhotoModeButton != null) {
				LivePhotoModeButton.Dispose ();
				LivePhotoModeButton = null;
			}

			if (PhotoButton != null) {
				PhotoButton.Dispose ();
				PhotoButton = null;
			}

			if (PreviewView != null) {
				PreviewView.Dispose ();
				PreviewView = null;
			}

			if (RecordButton != null) {
				RecordButton.Dispose ();
				RecordButton = null;
			}

			if (ResumeButton != null) {
				ResumeButton.Dispose ();
				ResumeButton = null;
			}
		}
	}
}
