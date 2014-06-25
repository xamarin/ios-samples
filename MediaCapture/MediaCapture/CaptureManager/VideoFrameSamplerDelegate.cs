//
// how to capture still images, video and audio using iOS AVFoundation and the AVCAptureSession
// 
// This sample handles all of the low-level AVFoundation and capture graph setup required to capture and save media.  This code also exposes the
// capture, configuration and notification capabilities in a more '.Netish' way of programming.  The client code will not need to deal with threads, delegate classes
// buffer management, or objective-C data types but instead will create .NET objects and handle standard .NET events.  The underlying iOS concepts and classes are detailed in 
// the iOS developer online help (TP40010188-CH5-SW2).
//
// https://developer.apple.com/library/mac/#documentation/AudioVideo/Conceptual/AVFoundationPG/Articles/04_MediaCapture.html#//apple_ref/doc/uid/TP40010188-CH5-SW2
//
// Enhancements, suggestions and bug reports can be sent to steve.millar@infinitekdev.com
//
using System;
using CoreGraphics;
using Foundation;
using UIKit;
using AVFoundation;
using CoreVideo;
using CoreMedia;

using CoreFoundation;
using System.Runtime.InteropServices;

namespace MediaCapture
{
	public class VideoFrameSamplerDelegate : AVCaptureVideoDataOutputSampleBufferDelegate 
	{ 	
		#region events
		public EventHandler<ImageCaptureEventArgs> ImageCaptured;
		private void onImageCaptured( UIImage image )
		{
			if ( ImageCaptured != null )
			{
				ImageCaptureEventArgs args = new ImageCaptureEventArgs();
				args.Image = image;
				args.CapturedAt = DateTime.Now;
				ImageCaptured( this, args );
			}
		}

		public EventHandler<CaptureErrorEventArgs> CaptureError;
		private void onCaptureError( string errorMessage )
		{
			if ( CaptureError != null )
			{
				try
				{
					CaptureErrorEventArgs args = new CaptureErrorEventArgs();	
					args.ErrorMessage = errorMessage;
					CaptureError(this, args);
				}
				catch 
				{
				}
			}
		}
		#endregion
		
		public override void DidOutputSampleBuffer
		(
			AVCaptureOutput captureOutput, 
			CMSampleBuffer sampleBuffer, 
			AVCaptureConnection connection
		)
		{
			try 
			{
				// render the image into the debug preview pane
				UIImage image = getImageFromSampleBuffer(sampleBuffer);
				
				// event the capture up
				onImageCaptured( image );
				
				// make sure AVFoundation does not run out of buffers
				sampleBuffer.Dispose ();
			} 
			catch (Exception ex)
			{
				string errorMessage =  string.Format("Failed to process image capture: {0}", ErrorHandling.GetExceptionDetailedText(ex) );
				onCaptureError( errorMessage );
			}
		}
		
		private UIImage getImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
		{
			// Get the CoreVideo image
			using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
			{
				// Lock the base address
				pixelBuffer.Lock (0);
				
				// Get the number of bytes per row for the pixel buffer
				var baseAddress = pixelBuffer.BaseAddress;
				int bytesPerRow = (int)pixelBuffer.BytesPerRow;
				int width = (int)pixelBuffer.Width;
				int height = (int)pixelBuffer.Height;
				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
				
				// Create a CGImage on the RGB colorspace from the configured parameter above
				using (var cs = CGColorSpace.CreateDeviceRGB ())
				using (var context = new CGBitmapContext (baseAddress,width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo) flags))
				using (var cgImage = context.ToImage ())
				{
					pixelBuffer.Unlock (0);
					return UIImage.FromImage (cgImage);
				}
			}
		}
		
		
	}
}

