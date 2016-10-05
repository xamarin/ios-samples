using System;
using System.Collections.Generic;

using AVFoundation;
using CoreGraphics;
using CoreImage;
using Foundation;
using UIKit;

namespace ManualCameraControls
{
	public partial class BracketedViewController : UIViewController
	{
		#region Private Variables

		List<UIImageView> Output = new List<UIImageView> ();
		int OutputIndex;

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

		#endregion

		#region Constructors

		public BracketedViewController (IntPtr handle) : base (handle)
		{
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Builds the output view.
		/// </summary>
		/// <returns>The output view.</returns>
		/// <param name="n">N.</param>
		UIImageView BuildOutputView (int n)
		{
			// Create a new image view controller
			var imageView = new UIImageView (new CGRect (CameraView.Frame.Width * n, 0, CameraView.Frame.Width, CameraView.Frame.Height));

			// Load a temp image
			imageView.Image = UIImage.FromFile ("Default-568h@2x.png");

			// Add a label
			var label = new UILabel (new CGRect (0, 20, CameraView.Frame.Width, 24));
			label.TextColor = UIColor.White;
			label.Text = string.Format ("Bracketed Image {0}", n);
			imageView.AddSubview (label);

			// Add to scrolling view
			ScrollView.AddSubview (imageView);

			// Return new image view
			return imageView;
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

			// Setup scrolling area
			ScrollView.ContentSize = new CGSize (CameraView.Frame.Width * 4f, CameraView.Frame.Height);

			// Add output views
			Output.Add (BuildOutputView (1));
			Output.Add (BuildOutputView (2));
			Output.Add (BuildOutputView (3));

			// Create preset settings
			var Settings = new AVCaptureBracketedStillImageSettings[] {
				AVCaptureAutoExposureBracketedStillImageSettings.Create (-2f),
				AVCaptureAutoExposureBracketedStillImageSettings.Create (0f),
				AVCaptureAutoExposureBracketedStillImageSettings.Create (2f)
			};

			OutputIndex = Settings.Length;

			// Wireup capture button
			CaptureButton.TouchUpInside += (sender, e) => {
				// Reset output index
				if (OutputIndex < Settings.Length)
					return;
				
				OutputIndex = 0;

				// Tell the camera that we are getting ready to do a bracketed capture
				ThisApp.StillImageOutput.PrepareToCaptureStillImageBracket (ThisApp.StillImageOutput.Connections [0], Settings, (ready, err) => {
					// Was there an error, if so report it
					if (err != null)
						Console.WriteLine ("Error: {0}", err.LocalizedDescription);
				});

				// Ask the camera to snap a bracketed capture
				ThisApp.StillImageOutput.CaptureStillImageBracket (ThisApp.StillImageOutput.Connections [0], Settings, (sampleBuffer, settings, err) => {
					// Convert raw image stream into a Core Image Image
					var imageData = AVCaptureStillImageOutput.JpegStillToNSData (sampleBuffer);
					var image = CIImage.FromData (imageData);

					// Display the resulting image
					Output [OutputIndex++].Image = UIImage.FromImage (image);

					// IMPORTANT: You must release the buffer because AVFoundation has a fixed number
					// of buffers and will stop delivering frames if it runs out.
					sampleBuffer.Dispose ();
				});
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
			}
		}

		/// <summary>
		/// Views the will disappear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillDisappear (bool animated)
		{
			// Stop display
			if (ThisApp.CameraAvailable)
				ThisApp.Session.StopRunning ();

			base.ViewWillDisappear (animated);

		}

		#endregion
	}
}
