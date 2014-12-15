using System;
using CoreGraphics;

using Foundation;
using UIKit;
using CoreVideo;
using AVFoundation;

namespace RosyWriter
{
	public partial class RosyWriterViewControllerUniversal : UIViewController
	{
		RosyWriterVideoProcessor videoProcessor;
		RosyWriterPreviewWindow oglView;

		UILabel frameRateLabel;
		UILabel dimensionsLabel;
		UILabel typeLabel;

		NSTimer timer;
		bool shouldShowStats;
		int backgroundRecordingID;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public RosyWriterViewControllerUniversal ()
			: base (UserInterfaceIdiomIsPhone ? "RosyWriterViewControllerUniversal_iPhone" : "RosyWriterViewControllerUniversal_iPad", null)
		{
		}

		// HACK: Updated method to match delegate in NSTimer.CreateRepeatingScheduledTimer()
		void UpdateLabels (NSTimer time)
		{
			if (shouldShowStats) {
				frameRateLabel.Text = String.Format ("{0:F} FPS", videoProcessor.VideoFrameRate);
				frameRateLabel.BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F);

				var dimension = videoProcessor.VideoDimensions;
				dimensionsLabel.Text = String.Format ("{0} x {1}", dimension.Width, dimension.Height);
				dimensionsLabel.BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F);

				// Turn the integer constant into something human readable
				var type = videoProcessor.VideoType;
				char [] code = new char [4];
				for (int i = 0; i < 4; i++){
					code [3-i] = (char) (type & 0xff);
					type = type >> 8;
				}
				var typeString = new String (code);

				typeLabel.Text = typeString;
				typeLabel.BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F);
			} else {
				frameRateLabel.Text = string.Empty;
				frameRateLabel.BackgroundColor = UIColor.Clear;

				dimensionsLabel.Text = string.Empty;
				dimensionsLabel.BackgroundColor = UIColor.Clear;

				typeLabel.Text = string.Empty;
				typeLabel.BackgroundColor = UIColor.Clear;
			}
		}

		UILabel LabelWithText (string text, float yPosition)
		{
			const float labelWidth = 200.0F;
			const float labelHeight = 40.0F;
			// HACK: Change this float into an nfloat
			nfloat xPosition = previewView.Bounds.Size.Width - labelWidth - 10;

			var label = new UILabel (new CGRect (xPosition, yPosition, labelWidth, labelHeight)) {
				Font = UIFont.SystemFontOfSize (36F),
				LineBreakMode = UILineBreakMode.WordWrap,
				TextAlignment = UITextAlignment.Right,
				TextColor = UIColor.White,
				BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F),
				Text = text
			};
			label.Layer.CornerRadius = 4f;

			return label;
		}

		void DeviceOrientationDidChange (NSNotification notification)
		{
			var orientation = UIDevice.CurrentDevice.Orientation;
			// Don't update the reference orientation when the device orientation is face up/down or unknown.
			if (UIDeviceOrientation.Portrait == orientation || (UIDeviceOrientation.LandscapeLeft == orientation || UIDeviceOrientation.LandscapeRight == orientation))
				videoProcessor.ReferenceOrientation = OrientationFromDeviceOrientation (orientation);
		}

		static AVCaptureVideoOrientation OrientationFromDeviceOrientation (UIDeviceOrientation orientation)
		{
			switch (orientation) {
			case UIDeviceOrientation.PortraitUpsideDown:
				return AVCaptureVideoOrientation.PortraitUpsideDown;
			case UIDeviceOrientation.Portrait:
				return AVCaptureVideoOrientation.Portrait;
			case UIDeviceOrientation.LandscapeLeft:
				return AVCaptureVideoOrientation.LandscapeLeft;
			case UIDeviceOrientation.LandscapeRight:
				return AVCaptureVideoOrientation.LandscapeRight;
			default:
				return (AVCaptureVideoOrientation) 0;
			}
		}

		void Cleanup ()
		{
			frameRateLabel.Dispose ();
			dimensionsLabel.Dispose ();
			typeLabel.Dispose ();

			var notificationCenter = NSNotificationCenter.DefaultCenter;
			notificationCenter.RemoveObserver (this, UIDevice.OrientationDidChangeNotification, UIApplication.SharedApplication);
			UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications ();

			notificationCenter.RemoveObserver (this, UIApplication.DidBecomeActiveNotification, UIApplication.SharedApplication);

			// Stop and tear down the capture session
			videoProcessor.StopAndTearDownCaptureSession ();
			videoProcessor.Dispose ();
		}

		#region Event Handler
		public void OnPixelBufferReadyForDisplay (CVImageBuffer imageBuffer)
		{
			// Don't make OpenGLES calls while in the backgroud.
			if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Background)
				oglView.DisplayPixelBuffer(imageBuffer);
		}

		public void OnToggleRecording (object sender, EventArgs e)
		{
			// UIBarButtonItem btn = (UIBarButtonItem)sender;

			// Wait for the recording to start/stop before re-enabling the record button.
			InvokeOnMainThread (() => btnRecord.Enabled = false);

			// The recordingWill/DidStop delegate methods will fire asynchronously in the response to this call.
			if (videoProcessor.IsRecording)
				videoProcessor.StopRecording ();
			else
				videoProcessor.StartRecording ();
		}

		public void OnApplicationDidBecomeActive (NSNotification notification)
		{
			// For performance reasons, we manually pause/resume the session when saving a recoding.
			// If we try to resume the session in the background it will fail. Resume the session here as well to ensure we will succeed.
			videoProcessor.ResumeCaptureSession ();
		}

		#region Video Processer Event handlers
		public void OnRecordingWillStart ()
		{
			InvokeOnMainThread (() => {
				btnRecord.Enabled = false;
				btnRecord.Title = "Stop";

				// Disable the idle timer while we are recording
				UIApplication.SharedApplication.IdleTimerDisabled = true;

				// Make sure we have time to finish saving the movie if the app is backgrounded during recording
				if (UIDevice.CurrentDevice.IsMultitaskingSupported)
					// HACK: Cast nint to int
					backgroundRecordingID = (int)UIApplication.SharedApplication.BeginBackgroundTask (() => {});
			});
		}

		public void OnRecordingDidStart ()
		{
			InvokeOnMainThread (() => btnRecord.Enabled = true);
		}

		public void OnRecordingWillStop ()
		{
			InvokeOnMainThread (() => {
				// Disable until saving to the camera roll is complete
				btnRecord.Title = "Record";
				btnRecord.Enabled = false;

				// Pause the capture session so the saving will be as fast as possible.
				// We resme the session in recordingDidStop
				videoProcessor.PauseCaptureSession ();
			});
		}

		public void OnRecordingDidStop ()
		{
			InvokeOnMainThread (() => {
				btnRecord.Enabled = true;

				UIApplication.SharedApplication.IdleTimerDisabled = false;

				videoProcessor.ResumeCaptureSession ();

				if (UIDevice.CurrentDevice.IsMultitaskingSupported)
				{
					UIApplication.SharedApplication.EndBackgroundTask (backgroundRecordingID);
					backgroundRecordingID = 0;
				}
			});
		}
		#endregion
		#endregion

		#region UIViewController Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Initialize the class responsible for managing AV capture session and asset writer
			videoProcessor = new RosyWriterVideoProcessor ();

			// Keep track of changes to the device orientation so we can update the video processor
			var notificationCenter = NSNotificationCenter.DefaultCenter;
			notificationCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, DeviceOrientationDidChange);

			UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications ();

			// Setup and start the capture session
			videoProcessor.SetupAndStartCaptureSession ();

			notificationCenter.AddObserver (UIApplication.DidBecomeActiveNotification, OnApplicationDidBecomeActive);

			oglView = new RosyWriterPreviewWindow(CGRect.Empty);

			// Our interface is always in portrait
			oglView.Transform = videoProcessor.TransformFromCurrentVideoOrientationToOrientation(AVCaptureVideoOrientation.Portrait);

			CGRect bounds = previewView.ConvertRectToView(previewView.Bounds, oglView);
			oglView.Bounds = bounds;
			oglView.Center = new CGPoint(previewView.Bounds.Size.Width / 2.0F, previewView.Bounds.Size.Height / 2.0F);

			previewView.AddSubview(oglView);

			// Set up labels
			shouldShowStats = true;

			frameRateLabel = LabelWithText (string.Empty, 10.0F);
			previewView.AddSubview (frameRateLabel);

			dimensionsLabel = LabelWithText (string.Empty, 54.0F);
			previewView.AddSubview (dimensionsLabel);

			typeLabel = LabelWithText (string.Empty, 90F);
			previewView.Add (typeLabel);

			// btnRecord Event Handler
			btnRecord.Clicked += OnToggleRecording;

			// Video Processor Event Handlers
			videoProcessor.RecordingDidStart += OnRecordingDidStart;
			videoProcessor.RecordingDidStop += OnRecordingDidStop;
			videoProcessor.RecordingWillStart += OnRecordingWillStart;
			videoProcessor.RecordingWillStop += OnRecordingWillStop;
			videoProcessor.PixelBufferReadyForDisplay += OnPixelBufferReadyForDisplay;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			timer = NSTimer.CreateRepeatingScheduledTimer (.25, UpdateLabels);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			timer.Invalidate ();
			timer.Dispose ();
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}
		#endregion
	}
}
