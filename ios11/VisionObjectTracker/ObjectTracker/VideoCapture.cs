using System;
using System.Linq;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using UIKit;

namespace ObjectTracker
{
	/// <summary>
	/// Handles the capturing of video from the device's rear camera
	/// </summary>
	public class VideoCapture : NSObject
	{
		/// <summary>
		/// The camera
		/// </summary>
		private AVCaptureDevice captureDevice;

		/// <summary>
		/// Perform the capture and image-processing pipeline on a background queue (note that 
		/// this is different than the Vision request
		/// </summary>
		DispatchQueue queue = new DispatchQueue("videoQueue");

		/// <summary>
		/// Used by `AVCaptureVideoPreviewLayer` in `ViewController`
		/// </summary>
		/// <value>The session.</value>
		public AVCaptureSession Session { get; }


		AVCaptureVideoDataOutput videoOutput = new AVCaptureVideoDataOutput();

		public IAVCaptureVideoDataOutputSampleBufferDelegate Delegate { get;  }

		public VideoCapture(IAVCaptureVideoDataOutputSampleBufferDelegate delegateObject)
		{
			Delegate = delegateObject;
			Session = new AVCaptureSession();
			SetupCamera();
		}

		/// <summary>
		/// Typical video-processing code. More advanced would allow user selection of camera, resolution, etc.
		/// </summary>
		private void SetupCamera()
		{
			var deviceDiscovery = AVCaptureDeviceDiscoverySession.Create(
				new AVCaptureDeviceType[] { AVCaptureDeviceType.BuiltInWideAngleCamera }, AVMediaType.Video, AVCaptureDevicePosition.Back);

			var device = deviceDiscovery.Devices.Last();
			if (device != null)
			{
				captureDevice = device;
				BeginSession();
			}
		}

		private void BeginSession()
		{
			try
			{
				var settings = new CVPixelBufferAttributes
				{
					PixelFormatType = CVPixelFormatType.CV32BGRA
				};
				videoOutput.WeakVideoSettings = settings.Dictionary;
				videoOutput.AlwaysDiscardsLateVideoFrames = true;
				videoOutput.SetSampleBufferDelegateQueue(Delegate, queue);

				Session.SessionPreset = AVCaptureSession.Preset1920x1080;
				Session.AddOutput(videoOutput);

				var input = new AVCaptureDeviceInput(captureDevice, out var err);
				if (err != null)
				{
					Console.Error.WriteLine("AVCapture error: " + err);
				}
				Session.AddInput(input);

				Session.StartRunning();
				Console.WriteLine("started AV capture session");
			}
			catch
			{
				Console.Error.WriteLine("error connecting to the capture device");
			}
		}

		/// <summary>
		/// This is an expensive call. Used by preview thumbnail.
		/// </summary>
		/// <returns>The (processed) video frame, as a UIImage.</returns>
		/// <param name="imageBuffer">The (processed) video frame.</param>
		public static UIImage ImageBufferToUIImage(CVPixelBuffer imageBuffer)
		{
			imageBuffer.Lock(CVPixelBufferLock.None);

			var baseAddress = imageBuffer.BaseAddress;
			var bytesPerRow = imageBuffer.BytesPerRow;

			var width = imageBuffer.Width;
			var height = imageBuffer.Height;

			var colorSpace = CGColorSpace.CreateDeviceRGB();
			var bitmapInfo = (uint)CGImageAlphaInfo.NoneSkipFirst | (uint)CGBitmapFlags.ByteOrder32Little;

			using (var context = new CGBitmapContext(baseAddress, width, height, 8, bytesPerRow, colorSpace, (CGImageAlphaInfo)bitmapInfo))
			{
				var quartzImage = context?.ToImage();
				imageBuffer.Unlock(CVPixelBufferLock.None);

				var image = new UIImage(quartzImage, 1.0f, UIImageOrientation.Up);

				return image;
			}
		}
	}
}
