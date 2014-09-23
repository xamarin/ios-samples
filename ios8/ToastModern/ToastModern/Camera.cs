using System;
using AVFoundation;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ToastModern
{
	public class Camera : Layer
	{
		AVCaptureVideoPreviewLayer CameraLayer {
			get {
				AVCaptureDevice defaultDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
				if (defaultDevice == null) {
					Console.WriteLine ("No camera available");
					return null;
				}

				AVCaptureDevice device = defaultDevice;

				NSError error;
				var captureDeviceInput = new AVCaptureDeviceInput (device, out error);

				var session = new AVCaptureSession ();
				session.BeginConfiguration ();
				session.SessionPreset = AVCaptureSession.PresetPhoto;
				session.AddInput (captureDeviceInput);
				session.CommitConfiguration ();
				session.StartRunning ();

				var cam = new AVCaptureVideoPreviewLayer (session);
				cam.Opaque = true;
				cam.Connection.AutomaticallyAdjustsVideoMirroring = false;
				cam.Connection.VideoMirrored = true;

				return cam;
			}
		}

		public Camera (Layer parent) : base (parent)
		{
			CALayer preview = CameraLayer;
			if (preview != null) {
				Layer.AddSublayer (preview);
				preview.Position = new CGPoint (160, 309);
				preview.Bounds = new CGRect (preview.Bounds.Location.X, preview.Bounds.Location.Y, 320, 320f * 16 / 9);
				preview.Transform = CATransform3D.MakeRotation ((nfloat)Math.PI, 0f, 1f, 0f);

				var previewMask = new CALayer ();
				Layer.AddSublayer (previewMask);

				previewMask.Position = new CGPoint (160, 260);
				previewMask.Bounds = new CGRect (previewMask.Bounds.Location.X, previewMask.Bounds.Location.Y, 320f, 320f);
				previewMask.BackgroundColor = CreateColor (0, 0, 0, 255);
				preview.Mask = previewMask;
			} else {
				Layer fakePreview = new Layer (this);
				fakePreview.LoadImage ("toast.jpg");
				fakePreview.Width = fakePreview.Height = 320;
				fakePreview.Y = Screen.GlobalScreen.Height * 0.5f - fakePreview.Height * 0.5f;
			}
		}
	}
}

