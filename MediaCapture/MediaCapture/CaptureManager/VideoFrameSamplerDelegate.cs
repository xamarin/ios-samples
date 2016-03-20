using System;

using UIKit;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreGraphics;

namespace MediaCapture {
	public class VideoFrameSamplerDelegate : AVCaptureVideoDataOutputSampleBufferDelegate {
		#region events
		public EventHandler<ImageCaptureEventArgs> ImageCaptured;

		void OnImageCaptured (UIImage image)
		{
			if (ImageCaptured != null) {
				var args = new ImageCaptureEventArgs {
					Image = image,
					CapturedAt = DateTime.Now
				};
				ImageCaptured (this, args);
			}
		}

		public EventHandler<CaptureErrorEventArgs> CaptureError;

		void OnCaptureError (string errorMessage )
		{
			if (CaptureError == null)
				return;

			try {
				var args = new CaptureErrorEventArgs {
					ErrorMessage = errorMessage
				};
				CaptureError (this, args);
			}
			catch (Exception e) {
				Console.WriteLine (e.Message);
			}
		}
		#endregion

		public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			try {
				// render the image into the debug preview pane
				UIImage image = getImageFromSampleBuffer (sampleBuffer);

				// event the capture up
				OnImageCaptured (image);

				// make sure AVFoundation does not run out of buffers
				sampleBuffer.Dispose ();
			}
			catch (Exception ex) {
				string exceptionText = ErrorHandling.GetExceptionDetailedText (ex);
				string errorMessage = $"Failed to process image capture: {exceptionText}";
				OnCaptureError (errorMessage);
			}
		}

		UIImage getImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
		{
			// Get the CoreVideo image
			using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
				// Lock the base address
				pixelBuffer.Lock (CVOptionFlags.None);

				// Get the number of bytes per row for the pixel buffer
				var baseAddress = pixelBuffer.BaseAddress;
				var bytesPerRow = (int) pixelBuffer.BytesPerRow;
				var width = (int) pixelBuffer.Width;
				var height = (int) pixelBuffer.Height;
				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;

				// Create a CGImage on the RGB colorspace from the configured parameter above
				using (var cs = CGColorSpace.CreateDeviceRGB ())
				using (var context = new CGBitmapContext (baseAddress,width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo) flags))
				using (var cgImage = context.ToImage ()) {
					pixelBuffer.Unlock (CVOptionFlags.None);
					return UIImage.FromImage (cgImage);
				}
			}
		}
	}
}

