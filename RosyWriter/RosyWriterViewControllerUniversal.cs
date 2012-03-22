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

	    void UpdateLabels ()
		{
			if (shouldShowStats){
				var frameRateString = string.Format ("{0} FPS", videoProcessor.VideoFrameRate.ToString ("F"));
				frameRateLabel.Text = frameRateString;
				frameRateLabel.BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F);
				
				var dimensionString = string.Format ("{0} x {0}", videoProcessor.VideoDimensions.Width, videoProcessor.VideoDimensions.Height);
				dimensionsLabel.Text = dimensionString;
				dimensionsLabel.BackgroundColor = UIColor.FromRGBA (0, 0, 0, .25F);
				
				// TODO: How to get a Human Readable value from the Video Type
				var type = BitConverter.GetBytes((uint)videoProcessor.VideoType);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(type); 
				var typeString = BitConverter.ToString(type, 0, 4);
				
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
			float labelWidth = 200.0F;
			float labelHeight = 40.0F;
			float xPosition = previewView.Bounds.Size.Width - labelWidth - 10;
			
			RectangleF labelFrame = new RectangleF (xPosition, yPosition, labelWidth, labelHeight);
			UILabel label = new UILabel (labelFrame) {
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
		
		private AVCaptureVideoOrientation OrientationFromDeviceOrientation (UIDeviceOrientation orientation)
		{
			AVCaptureVideoOrientation retOrientation;
			switch (orientation){
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
		public void On_PixelBufferReadyForDisplay (CVImageBuffer imageBuffer)
		{	
			// Don't make OpenGLES calls while in the backgroud.
			if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Background)
				oglView.DisplayPixelBuffer(imageBuffer);
		}
				
		public void On_ToggleRecording (object sender, EventArgs e)
		{
			// UIBarButtonItem btn = (UIBarButtonItem)sender;
			
			// Wait for the recording to start/stop before re-enabling the record button.
			InvokeOnMainThread (() => { btnRecord.Enabled = false; });
			
			// The recordingWill/DidStop delegate methods will fire asynchronously in the response to this call.
			if (videoProcessor.IsRecording)
				videoProcessor.StopRecording ();
			else
				videoProcessor.StartRecording ();	
		}
		
		public void On_ApplicationDidBecomeActive (NSNotification notification)
		{
			// For performance reasons, we manually pause/resume the session when saving a recoding.
			// If we try to resume the session in the background it will fail. Resume the session here as well to ensure we will succeed.
			videoProcessor.ResumeCaptureSession ();
		}
		
		#region Video Processer Event handlers
		public void On_RecordingWillStart ()
		{
			Console.WriteLine("Recording Will Start now");
			InvokeOnMainThread (() => {
				btnRecord.Enabled = false;
				btnRecord.Title = "Stop";
				
				// Disable the idle timer while we are recording
				UIApplication.SharedApplication.IdleTimerDisabled = true;
				
				// Make sure we have time to finish saving the movie if the app is backgrounded during recording
				if (UIDevice.CurrentDevice.IsMultitaskingSupported)
					backgroundRecordingID = UIApplication.SharedApplication.BeginBackgroundTask (() => {});
			});
		}
		
		public void On_RecordingDidStart ()
		{
			Console.WriteLine("Recording Did Start");
			InvokeOnMainThread (() => {
				btnRecord.Enabled = true;
			});
		}
		
		public void On_RecordingWillStop ()
		{
			Console.WriteLine("Recording Will Stop");
			InvokeOnMainThread (() => {
				// Disable until saving to the camera roll is complete
				btnRecord.Title = "Record";
				btnRecord.Enabled = false;
				
				// Pause the capture session so the saving will be as fast as possible.
				// We resme the session in recordingDidStop
				videoProcessor.PauseCaptureSession ();
			});
		}
		
		public void On_RecordingDidStop ()
		{
			Console.WriteLine("Recording Did Stop");
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
				
			RectangleF bounds = previewView.ConvertRectToView(previewView.Bounds, oglView);
			oglView.Bounds = bounds;
			oglView.Center = new PointF(previewView.Bounds.Size.Width / 2.0F, previewView.Bounds.Size.Height / 2.0F);
			
			previewView.AddSubview(oglView);
			
			Console.WriteLine("Added OGL View");
			
			// Set up labels
			shouldShowStats = true;
			
			frameRateLabel = LabelWithText (string.Empty, 10.0F);
			previewView.AddSubview (frameRateLabel);
			
			dimensionsLabel = LabelWithText (string.Empty, 54.0F);
			previewView.AddSubview (dimensionsLabel);
			
			typeLabel = LabelWithText (string.Empty, 90F);
			previewView.Add (typeLabel);
			
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
			return (toInterfaceOrientation == UIInterfaceOrientation.Portrait);
		}
		#endregion
	}
}

