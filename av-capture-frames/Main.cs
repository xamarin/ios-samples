//
// How to capture video frames from the camera as images using AVFoundation sample
//
// Based on Apple's echnical Q&A QA1702 sample
//
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AVFoundation;
using MonoTouch.CoreVideo;
using MonoTouch.CoreMedia;
using MonoTouch.CoreGraphics;

using MonoTouch.CoreFoundation;
using System.Runtime.InteropServices;

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
		public static UIImageView ImageView;
		AVCaptureSession session;
		OutputRecorder outputRecorder;
		DispatchQueue queue;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.MakeKeyAndVisible ();
			window.BackgroundColor = UIColor.Black;
			
			if (SetupCaptureSession ()){
				ImageView = new UIImageView (new RectangleF (20, 20, 280, 280));
				window.AddSubview (ImageView);				
			} else
				window.AddSubview (new UILabel (new RectangleF (20, 20, 200, 60)) { Text = "No input device" });
			
			return true;
		}
		
		bool SetupCaptureSession ()
		{
			// configure the capture session for low resolution, change this if your code
			// can cope with more data or volume
			session = new AVCaptureSession () {
				SessionPreset = AVCaptureSession.PresetMedium
			};
			
			// create a device input and attach it to the session
			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			var input = AVCaptureDeviceInput.FromDevice (captureDevice);
			if (input == null){
				Console.WriteLine ("No input device");
				return false;
			}
			session.AddInput (input);
			
			// create a VideoDataOutput and add it to the sesion
			var output = new AVCaptureVideoDataOutput () {
				VideoSettings = new AVVideoSettings (CVPixelFormatType.CV32BGRA),
				
				// If you want to cap the frame rate at a given speed, in this sample: 15 frames per second
				MinFrameDuration = new CMTime (1, 15)
			};
			
			// configure the output
			queue = new MonoTouch.CoreFoundation.DispatchQueue ("myQueue");
			outputRecorder = new OutputRecorder ();
			output.SetSampleBufferDelegateAndQueue (outputRecorder, queue);
			session.AddOutput (output);
			
			session.StartRunning ();
			return true;
		}
		
		public override void OnActivated (UIApplication application)
		{
		}
		
		public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate { 	
			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
			{
				var image = ImageFromSampleBuffer (sampleBuffer);

				// Do something with the image, we just stuff it in our main view.
				AppDelegate.ImageView.BeginInvokeOnMainThread (delegate {
					AppDelegate.ImageView.Image = image;
				});
			
				sampleBuffer.Dispose ();
			}
			
			UIImage ImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
			{
				// Get the CoreVideo image
				using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer){
					// Lock the base address
					pixelBuffer.Lock (0);
					
					// Get the number of bytes per row for the pixel buffer
					var baseAddress = pixelBuffer.BaseAddress;
					int bytesPerRow = pixelBuffer.BytesPerRow;
					int width = pixelBuffer.Width;
					int height = pixelBuffer.Height;
					var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
					
					// Create a CGImage on the RGB colorspace from the configured parameter above
					using (var cs = CGColorSpace.CreateDeviceRGB ())
					using (var context = new CGBitmapContext (baseAddress,width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo) flags)){
					using (var cgImage = context.ToImage ()){
						pixelBuffer.Unlock (0);
						return UIImage.FromImage (cgImage);
					}
					}
				}
			}
		}
	}
}

