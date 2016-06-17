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
	[Register ("FocusViewController")]
	partial class FocusViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView CameraView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel NoCamera { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider Position { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISegmentedControl Segments { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (CameraView != null) {
				CameraView.Dispose ();
				CameraView = null;
			}
			if (NoCamera != null) {
				NoCamera.Dispose ();
				NoCamera = null;
			}
			if (Position != null) {
				Position.Dispose ();
				Position = null;
			}
			if (Segments != null) {
				Segments.Dispose ();
				Segments = null;
			}
		}
	}
}
