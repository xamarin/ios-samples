using System;
using System.Timers;

using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;
using ObjCRuntime;
using System.Runtime.InteropServices;

namespace ManualCameraControls
{
	public partial class ExposureViewController : UIViewController
	{
		#region Private Variables

		bool Automatic = true;
		const float ExposureDurationPower = 5f;
		const float ExposureMinimumDuration = 1.0f / 1000.0f;

		#endregion

		#region Computed Properties

		/// <summary>
		/// Returns the delegate of the current running application
		/// </summary>
		/// <value>The this app.</value>
		public AppDelegate ThisApp {
			get {
				return (AppDelegate)UIApplication.SharedApplication.Delegate;
			}
		}

		/// <summary>
		/// Gets or sets the sample timer.
		/// </summary>
		/// <value>The sample timer.</value>
		public Timer SampleTimer { get; set; }

		#endregion

		#region Constructors

		public ExposureViewController (IntPtr handle) : base (handle)
		{
		}

		#endregion

		#region Private methods

		/// <summary>
		/// CMs the time get seconds.
		/// </summary>
		/// <returns>The time get seconds.</returns>
		/// <param name="time">Time.</param>
		float CMTimeGetSeconds (CMTime time)
		{
			return (float)time.Value / (float)time.TimeScale;
		}

		#endregion

		#region Override Methods

		/// <summary>
		/// Views the did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Hide no camera label
			NoCamera.Hidden = ThisApp.CameraAvailable;

			// Attach to camera view
			ThisApp.Recorder.DisplayView = CameraView;

			// Set min and max values
			Offset.MinValue = ThisApp.CaptureDevice.MinExposureTargetBias;
			Offset.MaxValue = ThisApp.CaptureDevice.MaxExposureTargetBias;

			Duration.MinValue = 0.001f;
			Duration.MaxValue = 1f;

			ISO.MinValue = ThisApp.CaptureDevice.ActiveFormat.MinISO;
			ISO.MaxValue = ThisApp.CaptureDevice.ActiveFormat.MaxISO;

			Bias.MinValue = ThisApp.CaptureDevice.MinExposureTargetBias;
			Bias.MaxValue = ThisApp.CaptureDevice.MaxExposureTargetBias;

			// Create a timer to monitor and update the UI
			SampleTimer = new Timer (5000);
			SampleTimer.Elapsed += (sender, e) => {
				// Update position slider
				Offset.BeginInvokeOnMainThread (() => {
					Offset.Value = ThisApp.Input.Device.ExposureTargetOffset;
				});

				Duration.BeginInvokeOnMainThread (() => {
					var newDurationSeconds = CMTimeGetSeconds (ThisApp.Input.Device.ExposureDuration);
					var minDurationSeconds = Math.Max (CMTimeGetSeconds (ThisApp.CaptureDevice.ActiveFormat.MinExposureDuration), ExposureMinimumDuration);
					var maxDurationSeconds = CMTimeGetSeconds (ThisApp.CaptureDevice.ActiveFormat.MaxExposureDuration);
					var p = (newDurationSeconds - minDurationSeconds) / (maxDurationSeconds - minDurationSeconds);
					Duration.Value = (float)Math.Pow (p, 1.0f / ExposureDurationPower);
				});

				ISO.BeginInvokeOnMainThread (() => {
					ISO.Value = ThisApp.Input.Device.ISO;
				});

				Bias.BeginInvokeOnMainThread (() => {
					Bias.Value = ThisApp.Input.Device.ExposureTargetBias;
				});
			};

			// Watch for value changes
			Segments.ValueChanged += (sender, e) => {
				NSError err;
				// Lock device for change
				if (ThisApp.CaptureDevice.LockForConfiguration (out err)) {

					// Take action based on the segment selected
					switch (Segments.SelectedSegment) {
					case 0:
					// Activate auto exposure and start monitoring position
						Duration.Enabled = false;
						ISO.Enabled = false;
						ThisApp.CaptureDevice.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
						SampleTimer.Start ();
						Automatic = true;
						break;
					case 1:
					// Lock exposure and allow the user to control the camera
						SampleTimer.Stop ();
						ThisApp.CaptureDevice.ExposureMode = AVCaptureExposureMode.Locked;
						Automatic = false;
						Duration.Enabled = false;
						ISO.Enabled = false;
						break;
					case 2:
					// Custom exposure and allow the user to control the camera
						SampleTimer.Stop ();
						ThisApp.CaptureDevice.ExposureMode = AVCaptureExposureMode.Custom;
						Automatic = false;
						Duration.Enabled = true;
						ISO.Enabled = true;
						break;
					}

					// Unlock device
					ThisApp.CaptureDevice.UnlockForConfiguration ();
				}
			};

			// Monitor position changes
			Duration.ValueChanged += (sender, e) => {

				// If we are in the automatic mode, ignore changes
				if (Automatic)
					return;

				// Calculate value
				var p = Math.Pow (Duration.Value, ExposureDurationPower);
				var minDurationSeconds = Math.Max (ThisApp.CaptureDevice.ActiveFormat.MinExposureDuration.Seconds, ExposureMinimumDuration);
				var maxDurationSeconds = ThisApp.CaptureDevice.ActiveFormat.MaxExposureDuration.Seconds;
				var newDurationSeconds = p * (maxDurationSeconds - minDurationSeconds ) + minDurationSeconds;

				NSError err;
				// Update Focus position
				if (ThisApp.CaptureDevice.LockForConfiguration (out err)) {
					ThisApp.CaptureDevice.LockExposure (CMTime.FromSeconds (newDurationSeconds, 1000 * 1000 * 1000), AVCaptureDevice.ISOCurrent, null);
					ThisApp.CaptureDevice.UnlockForConfiguration ();
				}
			};

			ISO.ValueChanged += (sender, e) => {
				// If we are in the automatic mode, ignore changes
				if (Automatic)
					return;

				NSError err;
				// Update Focus position
				if (ThisApp.CaptureDevice.LockForConfiguration (out err)) {
					ThisApp.CaptureDevice.LockExposure (ThisApp.CaptureDevice.ExposureDuration, ISO.Value, null);
					ThisApp.CaptureDevice.UnlockForConfiguration ();
				}
			};

			Bias.ValueChanged += (sender, e) => {
				NSError err;
				// Update Focus position
				if (ThisApp.CaptureDevice.LockForConfiguration (out err)) {
					ThisApp.CaptureDevice.SetExposureTargetBias (Bias.Value, null);
					ThisApp.CaptureDevice.UnlockForConfiguration ();
				}
			};
		}

		/// <summary>
		/// Views the will appear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// Start udating the display
			if (ThisApp.CameraAvailable) {
				// Remap to this camera view
				ThisApp.Recorder.DisplayView = CameraView;

				ThisApp.Session.StartRunning ();
				SampleTimer.Start ();
			}
		}

		/// <summary>
		/// Views the will disappear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillDisappear (bool animated)
		{
			// Stop display
			if (ThisApp.CameraAvailable) {
				SampleTimer.Stop ();
				ThisApp.Session.StopRunning ();
			}

			base.ViewWillDisappear (animated);
		}

		#endregion
	}
}
