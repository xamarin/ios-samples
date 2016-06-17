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
	[Register ("WhiteBalanceViewController")]
	partial class WhiteBalanceViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView CameraView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton GrayCardButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel NoCamera { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISegmentedControl Segments { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider Temperature { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider Tint { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CameraView != null) {
				CameraView.Dispose ();
				CameraView = null;
			}
			if (GrayCardButton != null) {
				GrayCardButton.Dispose ();
				GrayCardButton = null;
			}
			if (NoCamera != null) {
				NoCamera.Dispose ();
				NoCamera = null;
			}
			if (Segments != null) {
				Segments.Dispose ();
				Segments = null;
			}
			if (Temperature != null) {
				Temperature.Dispose ();
				Temperature = null;
			}
			if (Tint != null) {
				Tint.Dispose ();
				Tint = null;
			}
		}
	}
}
