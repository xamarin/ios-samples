using System;
using System.Timers;

using AVFoundation;
using Foundation;
using UIKit;

namespace ManualCameraControls
{
	public partial class FocusViewController : UIViewController
	{
		#region Private Variables

		NSError Error;
		bool Automatic = true;

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

		/// <summary>
		/// Initializes a new instance of the <see cref="ManualCameraControls.FocusViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public FocusViewController (IntPtr handle) : base (handle)
		{
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

			// Create a timer to monitor and update the UI
			SampleTimer = new Timer (5000);
			SampleTimer.Elapsed += (sender, e) => Position.BeginInvokeOnMainThread (() => {
				Position.Value = ThisApp.Input.Device.LensPosition;
			});

			// Watch for value changes
			Segments.ValueChanged += (sender, e) => {
				// Lock device for change
				if (ThisApp.CaptureDevice.LockForConfiguration (out Error)) {

					// Take action based on the segment selected
					switch (Segments.SelectedSegment) {
					case 0:
					// Activate auto focus and start monitoring position
						Position.Enabled = false;
						ThisApp.CaptureDevice.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
						SampleTimer.Start ();
						Automatic = true;
						break;
					case 1:
					// Stop auto focus and allow the user to control the camera
						SampleTimer.Stop ();
						ThisApp.CaptureDevice.FocusMode = AVCaptureFocusMode.Locked;
						Automatic = false;
						Position.Enabled = true;
						break;
					}

					// Unlock device
					ThisApp.CaptureDevice.UnlockForConfiguration ();
				}
			};

			// Monitor position changes
			Position.ValueChanged += (sender, e) => {
				// If we are in the automatic mode, ignore changes
				if (Automatic)
					return;

				// Update Focus position
				if (ThisApp.CaptureDevice.LockForConfiguration (out Error)) {
					ThisApp.CaptureDevice.SetFocusModeLocked (Position.Value, null);
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
