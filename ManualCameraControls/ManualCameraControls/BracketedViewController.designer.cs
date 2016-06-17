// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ManualCameraControls
{
	[Register ("BracketedViewController")]
	partial class BracketedViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView CameraView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton CaptureButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel NoCamera { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIScrollView ScrollView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CameraView != null) {
				CameraView.Dispose ();
				CameraView = null;
			}
			if (CaptureButton != null) {
				CaptureButton.Dispose ();
				CaptureButton = null;
			}
			if (NoCamera != null) {
				NoCamera.Dispose ();
				NoCamera = null;
			}
			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}
		}
	}
}
