// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BracketStripes
{
	[Register ("CameraViewController")]
	partial class CameraViewController
	{
		[Outlet]
		UIKit.UISegmentedControl bracketModeControl { get; set; }

		[Outlet]
		BracketStripes.CapturePreviewView cameraPreviewView { get; set; }

		[Outlet]
		UIKit.UIButton cameraShutterButton { get; set; }

		[Action ("BracketChangeDidChange:")]
		partial void BracketChangeDidChange (Foundation.NSObject sender);

		[Action ("CameraShutterDidPress:")]
		partial void CameraShutterDidPress (UIKit.UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (bracketModeControl != null) {
				bracketModeControl.Dispose ();
				bracketModeControl = null;
			}

			if (cameraPreviewView != null) {
				cameraPreviewView.Dispose ();
				cameraPreviewView = null;
			}

			if (cameraShutterButton != null) {
				cameraShutterButton.Dispose ();
				cameraShutterButton = null;
			}
		}
	}
}
