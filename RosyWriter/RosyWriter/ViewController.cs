using AVFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using RosyWriter.Helpers;
using System;
using UIKit;

namespace RosyWriter {
	public partial class ViewController : UIViewController {
		private RosyWriterVideoProcessor videoProcessor;

		private nint backgroundRecordingID;

		private bool shouldShowStats;

		private NSTimer timer;

		protected ViewController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			statisticView.Layer.CornerRadius = 4f;

			// Initialize the class responsible for managing AV capture session and asset writer
			videoProcessor = new RosyWriterVideoProcessor ();

			// Setup and start the capture session
			var isSupported = videoProcessor.SetupAndStartCaptureSession ();
			if (isSupported) {
				// Our interface is always in portrait
				previewView.Transform = videoProcessor.TransformFromCurrentVideoOrientationToOrientation (AVCaptureVideoOrientation.Portrait);
				previewView.Bounds = View.ConvertRectToView (View.Bounds, previewView);
				previewView.Center = new CGPoint (View.Bounds.Size.Width / 2f, View.Bounds.Size.Height / 2f);

				UIApplication.Notifications.ObserveDidBecomeActive (OnApplicationDidBecomeActive);

				// Set up labels
				shouldShowStats = true;

				// Video Processor Event Handlers
				videoProcessor.RecordingDidStart += OnRecordingDidStart;
				videoProcessor.RecordingDidStop += OnRecordingDidStop;
				videoProcessor.RecordingWillStart += OnRecordingWillStart;
				videoProcessor.RecordingWillStop += OnRecordingWillStop;
				videoProcessor.PixelBufferReadyForDisplay += OnPixelBufferReadyForDisplay;
			} else {
				errorLabel.Hidden = false;

				previewView.Hidden = true;
				recordButton.Enabled = false;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			timer = NSTimer.CreateRepeatingScheduledTimer (.25, UpdateLabels);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			if (timer != null) {
				timer.Invalidate ();
				timer.Dispose ();
				timer = null;
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			if (timer != null) {
				timer.Invalidate ();
				timer.Dispose ();
				timer = null;
			}

			if (videoProcessor != null) {
				videoProcessor.Dispose ();
				videoProcessor = null;
			}
		}

		private void UpdateLabels (NSTimer time)
		{
			statisticView.Hidden = !shouldShowStats;
			if (shouldShowStats) {
				statisticView.UpdateLabel (videoProcessor);
			}
		}

		#region Event Handler

		public void OnPixelBufferReadyForDisplay (CVImageBuffer imageBuffer)
		{
			// Don't make OpenGLES calls while in the backgroud.
			if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Background) {
				previewView.DisplayPixelBuffer (imageBuffer);
			}
		}

		partial void OnRecordButtonClicked (UIBarButtonItem sender)
		{
			// Wait for the recording to start/stop before re-enabling the record button.
			InvokeOnMainThread (() => recordButton.Enabled = false);

			// The recordingWill/DidStop delegate methods will fire asynchronously in the response to this call.
			if (videoProcessor.IsRecording) {
				videoProcessor.StopRecording ();
			} else {
				videoProcessor.StartRecording ();
			}
		}

		private void OnApplicationDidBecomeActive (object sender, NSNotificationEventArgs e)
		{
			// For performance reasons, we manually pause/resume the session when saving a recoding.
			// If we try to resume the session in the background it will fail. Resume the session here as well to ensure we will succeed.
			videoProcessor.ResumeCaptureSession ();
		}

		#endregion

		#region Video Processer Event handlers

		public void OnRecordingWillStart ()
		{
			InvokeOnMainThread (() => {
				recordButton.Enabled = false;
				recordButton.Title = "Stop";

				// Disable the idle timer while we are recording
				UIApplication.SharedApplication.IdleTimerDisabled = true;

				// Make sure we have time to finish saving the movie if the app is backgrounded during recording
				if (UIDevice.CurrentDevice.IsMultitaskingSupported) {
					backgroundRecordingID = UIApplication.SharedApplication.BeginBackgroundTask (() => {
						UIApplication.SharedApplication.EndBackgroundTask (backgroundRecordingID);
					});
				}
			});
		}

		public void OnRecordingDidStart ()
		{
			InvokeOnMainThread (() => recordButton.Enabled = true);
		}

		public void OnRecordingWillStop ()
		{
			InvokeOnMainThread (() => {
				// Disable until saving to the camera roll is complete
				recordButton.Title = "Record";
				recordButton.Enabled = false;

				// Pause the capture session so the saving will be as fast as possible.
				// We resme the session in recordingDidStop
				videoProcessor.PauseCaptureSession ();
			});
		}

		public void OnRecordingDidStop ()
		{
			InvokeOnMainThread (() => {
				recordButton.Enabled = true;
				UIApplication.SharedApplication.IdleTimerDisabled = false;

				videoProcessor.ResumeCaptureSession ();

				if (UIDevice.CurrentDevice.IsMultitaskingSupported) {
					UIApplication.SharedApplication.EndBackgroundTask (backgroundRecordingID);
					backgroundRecordingID = 0;
				}
			});
		}

		#endregion
	}
}
