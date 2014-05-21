//
// How to capture video frames from the camera as images using AVFoundation sample
//
// Based on Apple's technical Q&A QA1702 sample
//
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Foundation;
using UIKit;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreGraphics;

using CoreFoundation;
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
		UIViewController vc;
		AVCaptureSession session;
		OutputRecorder outputRecorder;
		DispatchQueue queue;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			ImageView = new UIImageView (new RectangleF (20, 20, 280, 280));
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

			vc = new UIViewController ();
			vc.View = ImageView;
			window.RootViewController = vc;

			window.MakeKeyAndVisible ();
			window.BackgroundColor = UIColor.Black;

			if (!SetupCaptureSession ())
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
			if (captureDevice == null){
				Console.WriteLine ("No captureDevice - this won't work on the simulator, try a physical device");
				return false;
			}
			//Configure for 15 FPS. Note use of LockForConigfuration()/UnlockForConfiguration()
			NSError error = null;
			captureDevice.LockForConfiguration(out error);
			if(error != null)
			{
				Console.WriteLine(error);
				captureDevice.UnlockForConfiguration();
				return false;
			}
			if(UIDevice.CurrentDevice.CheckSystemVersion(7,0))
				captureDevice.ActiveVideoMinFrameDuration = new CMTime (1,15);
			captureDevice.UnlockForConfiguration();


			var input = AVCaptureDeviceInput.FromDevice (captureDevice);
			if (input == null){
				Console.WriteLine ("No input - this won't work on the simulator, try a physical device");
				return false;
			}
			session.AddInput (input);
			
			// create a VideoDataOutput and add it to the sesion
			var output = new AVCaptureVideoDataOutput () {
				CompressedVideoSetting = new AVVideoSettingsCompressed ()
			};


			// configure the output
			queue = new CoreFoundation.DispatchQueue ("myQueue");
			outputRecorder = new OutputRecorder ();
			output.SetSampleBufferDelegate (outputRecorder, queue);
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
				try {
					var image = ImageFromSampleBuffer (sampleBuffer);
	
					// Do something with the image, we just stuff it in our main view.
					AppDelegate.ImageView.BeginInvokeOnMainThread (delegate {
						AppDelegate.ImageView.Image = image;
						AppDelegate.ImageView.Transform = CGAffineTransform.MakeRotation((float)Math.PI/2);

					});
			
					//
					// Although this looks innocent "Oh, he is just optimizing this case away"
					// this is incredibly important to call on this callback, because the AVFoundation
					// has a fixed number of buffers and if it runs out of free buffers, it will stop
					// delivering frames. 
					//	
					sampleBuffer.Dispose ();
				} catch (Exception e){
					Console.WriteLine (e);
				}
			}
			
			UIImage ImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
			{
				// Get the CoreVideo image
				using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer){
					// Lock the base address
					pixelBuffer.Lock (0);
					// Get the number of bytes per row for the pixel buffer
					var baseAddress = pixelBuffer.BaseAddress;
					int bytesPerRow = (int)pixelBuffer.BytesPerRow;
					int width = (int)pixelBuffer.Width;
					int height = (int) pixelBuffer.Height;
					var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
					// Create a CGImage on the RGB colorspace from the configured parameter above
					using (var cs = CGColorSpace.CreateDeviceRGB ())
					using (var context = new CGBitmapContext (baseAddress,width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo) flags))
					using (var cgImage = context.ToImage ()){
						pixelBuffer.Unlock (0);
						return UIImage.FromImage (cgImage);
					}
				}
			}
		}
	}
}

