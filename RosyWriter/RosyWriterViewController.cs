using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreVideo;
using MonoTouch.AVFoundation;
using MonoTouch.GLKit;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreMedia;
using MonoTouch.OpenGLES;
using OpenTK.Graphics.ES20;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace RosyWriter
{
	public partial class RosyWriterViewController : UIViewController
	{
		#region Properties
		
		#endregion
		
		#region Private Members
		RosyWriterVideoProcessor videoProcessor;
		RosyWriterPreviewWindow oglView;
		
		UILabel frameRateLabel;
		UILabel dimensionsLabel;
		UILabel typeLabel;
		
		NSTimer timer;
		bool shouldShowStats;
		int backgroundRecordingID;
		#endregion
		
		public RosyWriterViewController () 
			: base ("RosyWriterViewControlleriPhone", null)
		{
			
		}
		
		public void UpdateLabels ()
		{
			if (shouldShowStats)
			{
				var frameRateString = string.Format ("{0} FPS", videoProcessor.VideoFrameRate.ToString ("F"));
				frameRateLabel.Text = frameRateString;
				frameRateLabel.BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F);
				
				var dimensionString = string.Format ("{0} x {0}", videoProcessor.VideoDimensions.Width, videoProcessor.VideoDimensions.Height);
				dimensionsLabel.Text = dimensionString;
				dimensionsLabel.BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F);
				
				var type = videoProcessor.VideoType;
				var typeString = Enum.GetName (typeof(CMVideoCodecType), type);
				typeLabel.Text = typeString;
				typeLabel.BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F);
			}
			else
			{
				frameRateLabel.Text = string.Empty;	
				frameRateLabel.BackgroundColor = UIColor.Clear;
				
				dimensionsLabel.Text = string.Empty;
				dimensionsLabel.BackgroundColor = UIColor.Clear;
				
				typeLabel.Text = string.Empty;
				typeLabel.BackgroundColor = UIColor.Clear;				
			}
		}
		
		public UILabel LabelWithText (string text, float yPosition)
		{
			float labelWidth = 200.0F;
			float labelHeight = 40.0F;
			float xPosition = viewPreview.Bounds.Size.Width - labelWidth - 10;
			
			RectangleF labelFrame = new RectangleF (xPosition, yPosition, labelWidth, labelHeight);
			UILabel label = new UILabel (labelFrame);
			
			label.Font = UIFont.SystemFontOfSize (36F);
			label.LineBreakMode = UILineBreakMode.WordWrap;
			label.TextAlignment = UITextAlignment.Right;
			label.TextColor = UIColor.White;
			label.BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F);
			label.Layer.CornerRadius = 4F;
			label.Text = text;
			
			return label;
		}
		
		public void DeviceOrientationDidChange (NSNotification notification)
		{
			var orientation = UIDevice.CurrentDevice.Orientation;
			// Don't update the reference orientation when the device orientation is face up/down or unknown.
			if (UIDeviceOrientation.Portrait == orientation || (UIDeviceOrientation.LandscapeLeft == orientation || UIDeviceOrientation.LandscapeRight == orientation))
				videoProcessor.ReferenceOrientation = orientationFromDeviceOrientation (orientation);
		}
		
		private AVCaptureVideoOrientation orientationFromDeviceOrientation (UIDeviceOrientation orientation)
		{
			AVCaptureVideoOrientation retOrientation;
			switch (orientation)
			{
			case UIDeviceOrientation.PortraitUpsideDown:
				retOrientation = AVCaptureVideoOrientation.PortraitUpsideDown;
				break;
			case UIDeviceOrientation.Portrait:
				retOrientation = AVCaptureVideoOrientation.Portrait;
				break;
			case UIDeviceOrientation.LandscapeLeft:
				retOrientation = AVCaptureVideoOrientation.LandscapeLeft;
				break;
			case UIDeviceOrientation.LandscapeRight:
				retOrientation = AVCaptureVideoOrientation.LandscapeRight;
				break;
			default:
				retOrientation = (AVCaptureVideoOrientation)0;
				break;
			}
			
			return retOrientation;
		}
		
		public void Cleanup ()
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
		public void On_PixelBufferReadyForDisplay (object sender, CVImageBuffer imageBuffer)
		{	
			// Don't make OpenGLES calls while in the backgroud.
			if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Background)
				oglView.DisplayPixelBuffer(imageBuffer);
		}
				
		public void On_ToggleRecording (object sender, EventArgs e)
		{
			// UIBarButtonItem btn = (UIBarButtonItem)sender;
			
			// Wait for the recording to start/stop before re-enabling the record button.
			InvokeOnMainThread (() => {
				btnRecord.Enabled = false; });
			
			if (videoProcessor.IsRecording)
			{
				// The recordingWill/DidStop delegate methods will fire asynchronously in the response to this call.
				videoProcessor.StopRecording ();
			}
			else
			{
				videoProcessor.StartRecording ();	
			}
			
		}
		
		public void On_ApplicationDidBecomeActive (NSNotification notification)
		{
			// For performance reasons, we manually pause/resume the session when saving a recoding.
			// If we try to resume the session in the background it will fail. Resume the session here as well to ensure we will succeed.
			videoProcessor.ResumeCaptureSession ();
		}
		
		#region Video Processer Event handlers
		public void On_RecordingWillStart (object sender)
		{
			InvokeOnMainThread (() => 
			{
				btnRecord.Enabled = false;
				btnRecord.Title = "Stop";
				
				// Disable the idle timer while we are recording
				UIApplication.SharedApplication.IdleTimerDisabled = true;
				
				// Make sure we have time to finish saving the movie if the app is backgrounded during recording
				if (UIDevice.CurrentDevice.IsMultitaskingSupported)
					backgroundRecordingID = UIApplication.SharedApplication.BeginBackgroundTask (() => {});
			});
		}
		
		public void On_RecordingDidStart (object sender)
		{
			InvokeOnMainThread (() => {
				btnRecord.Enabled = true;
			});
		}
		
		public void On_RecordingWillStop (object sender)
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
		
		public void On_RecordingDidStop (object sender)
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
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			if (fromInterfaceOrientation == UIInterfaceOrientation.Portrait || 
			   fromInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || fromInterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
			{
				
			}
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
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
			Console.WriteLine("Finished Setting up AV");
			
			notificationCenter.AddObserver (UIApplication.DidBecomeActiveNotification, On_ApplicationDidBecomeActive);
			
			oglView = new RosyWriterPreviewWindow(RectangleF.Empty);
			
			// Our interface is always in portrait
			oglView.Transform = videoProcessor.TransformFromCurrentVideoOrientationToOrientation(AVCaptureVideoOrientation.Portrait);
				
			RectangleF bounds = viewPreview.ConvertRectToView(viewPreview.Bounds, oglView);
			oglView.Bounds = bounds;
			oglView.Center = new PointF(viewPreview.Bounds.Size.Width / 2.0F, viewPreview.Bounds.Size.Height / 2.0F);
			
			viewPreview.AddSubview(oglView);
			
			Console.WriteLine("Added OGL View");
			
			// Set up labels
			shouldShowStats = true;
			
			frameRateLabel = LabelWithText (string.Empty, 10.0F);
			viewPreview.AddSubview (frameRateLabel);
			
			dimensionsLabel = LabelWithText (string.Empty, 54.0F);
			viewPreview.AddSubview (dimensionsLabel);
			
			typeLabel = LabelWithText (string.Empty, 90F);
			viewPreview.Add (typeLabel);
			
			// btnRecord Event Handler
			btnRecord.Clicked += On_ToggleRecording;
			
			// Video Processor Event Handlers
			videoProcessor.RecordingDidStart += On_RecordingDidStart;
			videoProcessor.RecordingDidStop += On_RecordingDidStop;
			videoProcessor.RecordingWillStart += On_RecordingWillStart;
			videoProcessor.RecordingWillStop += On_RecordingWillStop;
			videoProcessor.PixelBufferReadyForDisplay += On_PixelBufferReadyForDisplay;
			
			Console.WriteLine("Finished OnDidLoad");
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			Cleanup ();
			
			ReleaseDesignerOutlets ();
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
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
		#endregion
	}
}

