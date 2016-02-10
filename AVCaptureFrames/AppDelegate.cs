using System;

using CoreGraphics;
using Foundation;
using UIKit;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreFoundation;

namespace avcaptureframes {
	public partial class AppDelegate : UIApplicationDelegate {
		public static UIImageView ImageView;
		UIViewController vc;
		AVCaptureSession session;
		OutputRecorder outputRecorder;
		DispatchQueue queue;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			ImageView = new UIImageView (new CGRect (20f, 20f, 280f, 280f));
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

			vc = new UIViewController {
				View = ImageView
			};

			window.RootViewController = vc;

			window.MakeKeyAndVisible ();
			window.BackgroundColor = UIColor.Black;

			if (!SetupCaptureSession ())
				window.AddSubview (new UILabel (new CGRect (20f, 20f, 200f, 60f)) {
					Text = "No input device"
				});

			return true;
		}

		bool SetupCaptureSession ()
		{
			// configure the capture session for low resolution, change this if your code
			// can cope with more data or volume
			session = new AVCaptureSession {
				SessionPreset = AVCaptureSession.PresetMedium
			};

			// create a device input and attach it to the session
			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType (AVMediaType.Video);
			if (captureDevice == null) {
				Console.WriteLine ("No captureDevice - this won't work on the simulator, try a physical device");
				return false;
			}
			//Configure for 15 FPS. Note use of LockForConigfuration()/UnlockForConfiguration()
			NSError error = null;
			captureDevice.LockForConfiguration (out error);
			if (error != null) {
				Console.WriteLine (error);
				captureDevice.UnlockForConfiguration ();
				return false;
			}

			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0))
				captureDevice.ActiveVideoMinFrameDuration = new CMTime (1, 15);
			captureDevice.UnlockForConfiguration ();

			var input = AVCaptureDeviceInput.FromDevice (captureDevice);
			if (input == null) {
				Console.WriteLine ("No input - this won't work on the simulator, try a physical device");
				return false;
			}

			session.AddInput (input);

			// create a VideoDataOutput and add it to the sesion
			var settings = new CVPixelBufferAttributes {
				PixelFormatType = CVPixelFormatType.CV32BGRA
			};
			using (var output = new AVCaptureVideoDataOutput { WeakVideoSettings = settings.Dictionary }) {
				queue = new DispatchQueue ("myQueue");
				outputRecorder = new OutputRecorder ();
				output.SetSampleBufferDelegate (outputRecorder, queue);
				session.AddOutput (output);
			}

			session.StartRunning ();
			return true;
		}

		public override void OnActivated (UIApplication application)
		{
		}

		public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate
		{
			public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
			{
				try
				{
					var image = ImageFromSampleBuffer (sampleBuffer);

					// Do something with the image, we just stuff it in our main view.
					ImageView.BeginInvokeOnMainThread(() => {
						TryDispose (ImageView.Image);
						ImageView.Image = image;
						ImageView.Transform = CGAffineTransform.MakeRotation (NMath.PI / 2);
					});
				}
				catch (Exception e) {
					Console.WriteLine (e);
				}
				finally {
					sampleBuffer.Dispose ();
				}
			}

			UIImage ImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
			{
				// Get the CoreVideo image
				using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
					// Lock the base address
					pixelBuffer.Lock ((CVPixelBufferLock)0);
					// Get the number of bytes per row for the pixel buffer
					var baseAddress = pixelBuffer.BaseAddress;
					var bytesPerRow = (int)pixelBuffer.BytesPerRow;
					var width = (int)pixelBuffer.Width;
					var height = (int)pixelBuffer.Height;
					var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
					// Create a CGImage on the RGB colorspace from the configured parameter above
					using (var cs = CGColorSpace.CreateDeviceRGB ()) {
						using (var context = new CGBitmapContext (baseAddress, width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo)flags)) {
							using (CGImage cgImage = context.ToImage ()) {
								pixelBuffer.Unlock ((CVPixelBufferLock)0);
								return UIImage.FromImage (cgImage);
							}
						}
					}
				}
			}

			void TryDispose (IDisposable obj)
			{
				if (obj != null)
					obj.Dispose ();
			}
		}
	}
}

