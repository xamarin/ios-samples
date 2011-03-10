//
// How to capture video frames from the camera as images using AVFoundation sample
//
// Based on Apple's echnical Q&A QA1702 sample
//
//
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AVFoundation;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreVideo;
using MonoTouch.CoreMedia;

namespace avcaptureframes
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	public partial class AppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.MakeKeyAndVisible ();

			SetupCaptureSession ();
			return true;
		}
		
		void SetupCaptureSession ()
		{
			// configure the capture session for low resolution, change this if your code
			// can cope with more data or volume
			var session = new AVCaptureSession () {
				SessionPreset = "AVCaptureSessionPresetMedium"
			};
			
			// create a device input and attach it to the session
			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			var input = AVCaptureDeviceInput.FromDevice (captureDevice, IntPtr.Zero);
			if (input == null){
				// handle error here;
			}
			session.AddInput (input);
			
			// create a VideoDataOutput and add it to the sesion
			var output = new AVCaptureVideoDataOutput () {
				VideoSettings = NSDictionary.FromObjectAndKey (new NSNumber (CVPixelFormatType.CV32BGRA), CVPixelBuffer.PixelFormatTypeKey),
			};
			output.MinFrameDuration = CMTime. 
			session.AddOutput (output);
			
			// configure the output
			var queue = new DispatchQueue ("myQueue");
			
#if MONOTOUCH_4
			output.SetSampleBufferDelegateAndQueue (new OutputRecorder (), queue);
#else
			output.SetSampleBufferDelegatequeue (new OutputRecorder (), queue.Handle);
#endif
			
			// If you want to cap the frame rate to say, 15 frames per second, use this.
#if MONOTOUCH_4
			// new API in 4.0
			CMTime min = new CMTime (1, 15);
#else
			CMTime min = new CMTime () {
				min.TimeFlags = 1,
				min.Value = 1,
				min.TimeScale = 15
			};
#endif
			output.MinFrameDuration = min;
			
			session.StartRunning ();
		}
		
		public override void OnActivated (UIApplication application)
		{
		}
		
		public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate {
			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
			{
				var image = ImageFromSampleBuffer (sampleBuffer);
				// Do something with the image
			}
			
			UIImage ImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
			{
				
			}
		}
	}
}

