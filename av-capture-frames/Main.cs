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
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.MakeKeyAndVisible ();
			
			if (SetupCaptureSession ()){
				ImageView = new UIImageView (new RectangleF (20, 20, 280, 280));
				window.AddSubview (ImageView);				
			} else {
				window.AddSubview (new UILabel (new RectangleF (20, 20, 200, 60)) { Text = "No input device" });
			}
			
			return true;
		}
		
		OutputRecorder or;
		AVCaptureSession session;
		
		bool SetupCaptureSession ()
		{
			// configure the capture session for low resolution, change this if your code
			// can cope with more data or volume
			session = new AVCaptureSession () {
				SessionPreset = "AVCaptureSessionPresetMedium"
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
			var queue = new MonoTouch.CoreFoundation.DispatchQueue ("myQueue");
			or = new OutputRecorder ();
			output.SetSampleBufferDelegateAndQueue (or, queue);
			session.AddOutput (output);
			
			session.StartRunning ();
			return true;
		}
		
		public override void OnActivated (UIApplication application)
		{
		}
		
		public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate {
                [DllImport (MonoTouch.Constants.CoreFoundationLibrary, CharSet=CharSet.Unicode)]
                internal extern static IntPtr CFRelease (IntPtr obj);

			
			[Export ("captureOutput:didOutputSampleBuffer:fromConnection:")]
			public void Foo (AVCaptureOutput a, CMSampleBuffer b, AVCaptureConnection c)
			{
				Console.WriteLine ("here2 {0}", b.GetType ());
				b.Dispose ();
				//a.Dispose ();
				              
			}
			
			/*
			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
			{
				var image = ImageFromSampleBuffer (sampleBuffer);

				// Do something with the image, we just stuff it in our main view.
				AppDelegate.ImageView.BeginInvokeOnMainThread (delegate {
					AppDelegate.ImageView.Image = image;
				});
				captureOutput.Dispose ();
				sampleBuffer.Dispose ();
				connection.Dispose ();
			}
			*/
			
			UIImage ImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
			{
				// Get the CoreVideo image
				using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer){
					
				
				// Lock the base address
				var r = pixelBuffer.Lock (0);
				Console.WriteLine ("The return is: {0} on this handle: {1}", r, pixelBuffer.Handle);
				
				// Get the number of bytes per row for the pixel buffer
				var baseAddress = pixelBuffer.BaseAddress;
				int bytesPerRow = pixelBuffer.BytesPerRow;
				int width = pixelBuffer.Width;
				int height = pixelBuffer.Height;
				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
				
				Console.WriteLine ("Base={0} bytes={1} width={2} height={3} flags={4}", baseAddress,bytesPerRow, width,height, flags);
				// Create a CGImage on the RGB colorspace from the configured parameter above
				using (var cs = CGColorSpace.CreateDeviceRGB ())
				using (var context = new CGBitmapContext (baseAddress,width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo) flags)){
					Console.WriteLine ("Context is: {0}", context.Handle);
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

