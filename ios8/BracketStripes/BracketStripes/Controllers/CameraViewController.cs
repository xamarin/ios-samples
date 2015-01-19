using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;

namespace BracketStripes
{
	public partial class CameraViewController : UIViewController
	{
		private AVCaptureSession captureSession;
		private AVCaptureDevice captureDevice;
		private AVCaptureDeviceFormat captureDeviceFormat;
		private AVCaptureStillImageOutput stillImageOutput;
		private nuint maxBracketCount;
		private List<AVCaptureBracketedStillImageSettings> bracketSettings;
		private StripedImage imageStripes;
		private ImageViewController imageViewController;
		private UINavigationController navigationImageViewController;

		public bool UserInterfaceEnabled {
			get {
				return cameraShutterButton.Enabled;
			}

			set {
				cameraShutterButton.Enabled = bracketModeControl.Enabled = value;
			}
		}

		private List <AVCaptureBracketedStillImageSettings> ExposureBrackets {
			get {
				var brackets = new List <AVCaptureBracketedStillImageSettings> ();

				nuint fixedBracketCount = 3;
				var biasValues = new float[] { -2f, 0f, 2f };

				for (nuint index = 0; index < Math.Min (fixedBracketCount, maxBracketCount); index++) {
					float biasValue = biasValues [index];
					brackets.Add (AVCaptureAutoExposureBracketedStillImageSettings.Create (biasValue));
				}

				return brackets;
			}
		}

		private List <AVCaptureBracketedStillImageSettings> DurationISOBrackets {
			get {
				var brackets = new List <AVCaptureBracketedStillImageSettings> ();

				Console.WriteLine ("Camera device ISO range: [{0:##}, {1:##}]",
					captureDeviceFormat.MinISO, captureDeviceFormat.MaxISO);

				Console.WriteLine ("Camera device Duration range: [{0}, {1}]",
					captureDeviceFormat.MinExposureDuration.Seconds, captureDeviceFormat.MaxExposureDuration.Seconds);

				nuint fixedBracketCount = 3;
				var ISOValues = new float[] { 50f, 60f, 500f };
				var durationSecondsValues = new float[] { 0.25f, 0.05f, 0.005f };

				for (nuint index = 0; index < Math.Min (fixedBracketCount, maxBracketCount); index++) {
					float ISO = (float)Clamp (ISOValues [index], captureDeviceFormat.MinISO, captureDeviceFormat.MaxISO);
					double durationSeconds = Clamp (durationSecondsValues [index],
						                         captureDeviceFormat.MinExposureDuration.Seconds, captureDeviceFormat.MaxExposureDuration.Seconds);
					var duration = CMTime.FromSeconds (durationSeconds, 483);

					brackets.Add (AVCaptureManualExposureBracketedStillImageSettings.Create (duration, ISO));
				}

				return brackets;
			}
		}

		public CameraViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			UserInterfaceEnabled = false;

			StartCameraWithCompletionHandler ((success, error) => {
				if (success)
					UserInterfaceEnabled = true;
				else
					Console.WriteLine ("This error should be handled appropriately in your app -- start camera completion: {0}", error.LocalizedDescription);
			});
		}

		partial void CameraShutterDidPress (UIKit.UIButton sender)
		{
			if (!captureSession.Running)
				return;

			UserInterfaceEnabled = false;

			PerformBrackedCaptureWithCompletionHandler ((image) => {
				imageViewController = new ImageViewController (image) {
					ImageViewControllerDidFinish = ImageViewControllerDidFinish,
					Title = "Bracket Viewer Title"
				};

				navigationImageViewController = new UINavigationController (imageViewController) {
					ModalTransitionStyle = UIModalTransitionStyle.CoverVertical
				};

				PresentViewController (navigationImageViewController, true, null);
			});
		}

		partial void BracketChangeDidChange (NSObject sender)
		{
			UserInterfaceEnabled = false;

			PrepareBracketsWithCompletionHandler ((success, error) => {
				UserInterfaceEnabled = true;
			});
		}

		private AVCaptureDevice CameraDeviceForPosition (AVCaptureDevicePosition position)
		{
			foreach (var device in AVCaptureDevice.Devices)
				if (device.Position == position)
					return device;

			return null;
		}

		private void ShowErrorMessage (string message, string title)
		{
			UIAlertView alert = new UIAlertView (title, message, null, "OK", null);
			alert.Show ();
		}

		private void PerformBrackedCaptureWithCompletionHandler (Action<UIImage> completion)
		{
			int todo = bracketSettings.Count;
			int failed = 0;

			Console.WriteLine ("Performing bracketed capture: {0}", bracketSettings);
			var connection = stillImageOutput.ConnectionFromMediaType (AVMediaType.Video);

			stillImageOutput.CaptureStillImageBracket (connection, bracketSettings.ToArray (), (sampleBuffer, stillImageSettings, error) => {
				--todo;

				if (error == null) {
					Console.WriteLine ("Bracket {0}", stillImageSettings);
					imageStripes.AddSampleBuffer (sampleBuffer);
				} else {
					Console.WriteLine ("This error should be handled appropriately in your app -- Bracket {0} ERROR: {1}", stillImageSettings, error.LocalizedDescription);
					++failed;
				}

				if (todo == 0) {
					Console.WriteLine ("All {0} bracket(s) have been captured {1} error.", bracketSettings.Count, (failed != 0) ? @"with" : @"without");

					UIImage image = (failed == 0) ? imageStripes.ImageWithOrientation (UIImageOrientation.Right) : null;
					InvokeOnMainThread (() => completion (image));
				}
			});
		}

		private void StartCameraWithCompletionHandler (Action<bool, NSError> completion)
		{
			captureSession = new AVCaptureSession ();
			captureSession.BeginConfiguration ();
			captureDevice = CameraDeviceForPosition (AVCaptureDevicePosition.Back);

			if (captureDevice == null) {
				string message = "Error message back camera - not found";
				string title = "Error";
				ShowErrorMessage (message, title);
				return;
			}

			NSError error;
			AVCaptureDeviceInput deviceInput = AVCaptureDeviceInput.FromDevice (captureDevice, out error);
			if (deviceInput == null) {
				Console.WriteLine ("This error should be handled appropriately in your app -- obtain device input: {0}", error);

				string message = "Error message back camera - can't open.";
				string title = "Error";
				ShowErrorMessage (message, title);
				return;
			}

			captureSession.AddInput (deviceInput);
			stillImageOutput = new AVCaptureStillImageOutput ();

			//Or instead of JPEG, we can use one of the following pixel formats: BGRA, 420f output
			stillImageOutput.OutputSettings = new NSDictionary (AVVideo.CodecKey, AVVideo.CodecJPEG);
			captureSession.AddOutput (stillImageOutput);
			cameraPreviewView.ConfigureCaptureSession (captureSession, stillImageOutput);
			captureSession.SessionPreset = AVCaptureSession.PresetPhoto;

			captureDeviceFormat = captureDevice.ActiveFormat;
			captureSession.CommitConfiguration ();
			captureSession.StartRunning ();
			maxBracketCount = stillImageOutput.MaxBracketedCaptureStillImageCount;
			PrepareBracketsWithCompletionHandler (completion);
		}

		private void PrepareBracketsWithCompletionHandler (Action<bool, NSError> completion)
		{
			switch (bracketModeControl.SelectedSegment) {
			case 0:
				Console.WriteLine ("Configuring auto-exposure brackets...");
				bracketSettings = ExposureBrackets;
				break;
			case 1:
				Console.WriteLine ("Configuring duration/ISO brackets...");
				bracketSettings = DurationISOBrackets;
				break;
			}

			CMVideoDimensions dimesnions = captureDevice.ActiveFormat.HighResolutionStillImageDimensions;
			var dimensions = new CGSize (dimesnions.Width, dimesnions.Height);

			if (imageStripes != null)
				imageStripes.Dispose ();

			imageStripes = new StripedImage (dimensions, (int)dimensions.Width / 12, (int)bracketSettings.Count);
			Console.WriteLine ("Warming brackets: {0}", bracketSettings.Count);
			AVCaptureConnection connection = stillImageOutput.ConnectionFromMediaType (AVMediaType.Video);
			stillImageOutput.PrepareToCaptureStillImageBracket (connection, bracketSettings.ToArray (), (success, error) => {
				completion (success, error);
			});
		}

		private double Clamp (float value, double lo, double hi)
		{
			return Math.Max (lo, Math.Min (hi, value));
		}

		private void ImageViewControllerDidFinish (ImageViewController controller)
		{
			controller.DismissViewController (true, () => {
				UserInterfaceEnabled = true;
			});
		}
	}
}
