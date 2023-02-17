using System;
using UIKit;
using AVFoundation;
using Foundation;
using CoreGraphics;
using CoreAnimation;
using CoreFoundation;
using CoreMedia;
using System.Linq;
using CoreVideo;
using ImageIO;

namespace BreakfastFinder {
	[Register ("ViewController")]
	public class ViewController : UIViewController, IAVCaptureVideoDataOutputSampleBufferDelegate {
		public ViewController (IntPtr handle) : base (handle) { }

		protected CGSize bufferSize = CGSize.Empty;
		protected CALayer rootLayer = null;

		public virtual UIView previewView { get; }
		AVCaptureSession session = new AVCaptureSession ();
		AVCaptureVideoPreviewLayer previewLayer = null;
		AVCaptureVideoDataOutput videoDataOutput = new AVCaptureVideoDataOutput ();

		DispatchQueue videoDataOutputQueue = new DispatchQueue ("videoDataOutputQueue");

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			try {
				SetupAVCapture ();
			} catch {
				TeardownAVCapture ();
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		protected virtual void SetupAVCapture ()
		{
			AVCaptureDeviceInput deviceInput;

			// Select a video device, make an input
			var videoDevice = AVCaptureDeviceDiscoverySession.Create (
				new AVCaptureDeviceType [] { AVCaptureDeviceType.BuiltInWideAngleCamera },
				AVMediaType.Video,
				AVCaptureDevicePosition.Back
			).Devices.FirstOrDefault ();

			deviceInput = new AVCaptureDeviceInput (videoDevice, out NSError error);
			if (error != null) {
				Console.WriteLine ($"Could not create video device input: {error.LocalizedDescription}");
				return;
			}

			session.BeginConfiguration ();
			session.SessionPreset = AVCaptureSession.Preset640x480; // Model image size is smaller

			// Add a video input
			if (!session.CanAddInput (deviceInput)) {
				Console.WriteLine ("Could not add video device input to the session");
				session.CommitConfiguration ();
				return;
			}
			session.AddInput (deviceInput);

			if (session.CanAddOutput (videoDataOutput)) {
				session.AddOutput (videoDataOutput);
				// Add a video data ouptut
				videoDataOutput.AlwaysDiscardsLateVideoFrames = true;
				videoDataOutput.WeakVideoSettings = new NSDictionary (CVPixelBuffer.PixelFormatTypeKey, CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange);
				videoDataOutput.SetSampleBufferDelegateQueue (this, videoDataOutputQueue);
			} else {
				Console.WriteLine ("Could not add video data output to the session");
				session.CommitConfiguration ();
				return;
			}

			var captureConnection = videoDataOutput.ConnectionFromMediaType (AVMediaType.Video);
			// Always process the frames
			captureConnection.Enabled = true;
			videoDevice.LockForConfiguration (out NSError error2);
			if (error2 == null) {
				var formatDescription = videoDevice.ActiveFormat.FormatDescription as CMVideoFormatDescription;
				CMVideoDimensions dimensions = formatDescription.Dimensions;
				bufferSize.Width = dimensions.Width;
				bufferSize.Height = dimensions.Height;
				videoDevice.UnlockForConfiguration ();
			} else {
				Console.WriteLine ($"{error2.LocalizedDescription}");
			}
			session.CommitConfiguration ();
			previewLayer = AVCaptureVideoPreviewLayer.FromSession (session);
			previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
			rootLayer = previewView.Layer;
			previewLayer.Frame = rootLayer.Bounds;
			rootLayer.AddSublayer (previewLayer);
		}

		protected void StartCaptureSession ()
		{
			session.StartRunning ();
		}

		void TeardownAVCapture ()
		{
			previewLayer.RemoveFromSuperLayer ();
			previewLayer = null;
		}

		protected CGImagePropertyOrientation ExifOrientationFromDeviceOrientation ()
		{
			UIDeviceOrientation curDeviceOrientation = UIDevice.CurrentDevice.Orientation;
			CGImagePropertyOrientation exifOrientation;

			switch (curDeviceOrientation) {
			case UIDeviceOrientation.PortraitUpsideDown:
				exifOrientation = CGImagePropertyOrientation.Left;
				break;
			case UIDeviceOrientation.LandscapeLeft:
				exifOrientation = CGImagePropertyOrientation.UpMirrored;
				break;
			case UIDeviceOrientation.LandscapeRight:
				exifOrientation = CGImagePropertyOrientation.Down;
				break;
			case UIDeviceOrientation.Portrait:
				exifOrientation = CGImagePropertyOrientation.Up;
				break;
			default:
				exifOrientation = CGImagePropertyOrientation.Left;
				break;
			}

			return exifOrientation;
		}
	}
}
