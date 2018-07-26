using System;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using Foundation;
using UIKit;
using Vision;

namespace ObjectTracker
{
	/// <summary>
	/// Delegate-object for `VideoCapture`: Converts the sample buffer into a `CVPixelBuffer` processed for video analysis
	/// </summary>
	public class VideoCaptureDelegate : NSObject, IAVCaptureVideoDataOutputSampleBufferDelegate
	{

		DateTime lastAnalysis = DateTime.Now;  // controlling the pace of the machine vision analysis
		TimeSpan pace = new TimeSpan(0, 0, 0, 0, 333); // in milliseconds, classification will not repeat faster than this value

		/// <summary>
		/// Keep a single context around, to avoid per-frame allocation
		/// </summary>
		CIContext context = CIContext.Create();


		CIAffineTransform rotateTransform;
		CIAffineTransform cropTransform;

		/// <summary>
		/// Other filters (contrast, dilation, threshold, etc.) could be added, although it's not clear if 
		/// doing so benefits Vision / CoreML object recognition / tracking
		/// </summary>
		CIEdges edgeDetector;

		CVPixelBuffer resultBuffer;

		public event EventHandler<EventArgsT<CVPixelBuffer>> FrameCaptured = delegate {};
 
		public VideoCaptureDelegate(EventHandler<EventArgsT<CVPixelBuffer>> callback)
		{
			this.FrameCaptured = callback;
		}

		[Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
		public void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			try
			{
				var currentDate = DateTime.Now;
				// control the pace of the machine vision to protect battery life
				if (currentDate - lastAnalysis >= pace)
				{
					lastAnalysis = currentDate;
				}
				else
				{
					return; // don't run the classifier more often than we need
				}
				// Crop and resize the image data.
				// Note, this uses a Core Image pipeline that could be appended with other pre-processing.
				// If we don't want to do anything custom, we can remove this step and let the Vision framework handle
				// crop and resize as long as we are careful to pass the orientation properly.
				using (var croppedBuffer = CroppedSampleBuffer(sampleBuffer))
				{

					if (croppedBuffer == null)
					{
						return;
					}
					FrameCaptured(this, new EventArgsT<CVPixelBuffer>(croppedBuffer));
				}
			}
			finally
			{
				sampleBuffer.Dispose();
			}
		}

		bool alreadySet = false;

		public CVPixelBuffer CroppedSampleBuffer(CMSampleBuffer sampleBuffer)
		{
			var imageBuffer = sampleBuffer.GetImageBuffer();
			if (imageBuffer == null)
			{
				throw new ArgumentException("Cannot convert to CVImageBuffer");
			}

			// Only doing these calculations once for efficiency.
			// If the incoming images could change orientation or size during a session, this would need to be reset when that happens.
			if (!alreadySet)
			{
				alreadySet = true;

				var imageSize = imageBuffer.EncodedSize;

				/*
				 Incoming image size is set in VideoCapture.BeginSession as AVCaptureSession.Preset1920x1080;
				 Which, buffer-wise, is always captured landscape-style, but info.plist specifies that this 
				 app runs only in portrait. Therefore, the buffer is always sideways, i.e., `imageSize == [Width: 1920, Height: 1080]`

				 Since our UI blurs out the top and bottom of the image, what we're interested in is the middle 
				 3/5 of the long side, and the entirety of the 1080 (short side), rotated 90 degrees anti-clockwise.

				 To get good alignment, this also requires some manual tweaking (LayoutMargins?), which probably changes
				 between hardware
				*/

				var rotatedSize = new CGSize(imageSize.Height, imageSize.Width);

				var shorterSide = rotatedSize.Width < rotatedSize.Height ? rotatedSize.Width : rotatedSize.Height;

				rotateTransform = new CIAffineTransform
				{
					Transform = new CGAffineTransform(0, -1, 1, 0, 0, shorterSide)
				};

				cropTransform = new CIAffineTransform
				{
					Transform = CGAffineTransform.MakeTranslation(0, (int) (1920.0 / 5) + 60) // Translate down past the cropped area + manual tweak
				};

				edgeDetector = new CIEdges();
			}

			// Convert to CIImage because it is easier to manipulate
			var ciImage = CIImage.FromImageBuffer(imageBuffer);

			rotateTransform.Image = ciImage;
			cropTransform.Image = rotateTransform.OutputImage;
			edgeDetector.Image = cropTransform.OutputImage;

			var cropped = edgeDetector.OutputImage;


			// Note that the above pipeline could be easily appended with other image manipulations.
			// For example, to change the image contrast, detect edges, etc. It would be most efficient to handle all of
			// the image manipulation in a single Core Image pipeline because it can be hardware optimized.

			// Only need to create this buffer one time and then we can reuse it for every frame
			if (resultBuffer == null || resultBuffer.Handle == IntPtr.Zero)
			{
				var targetSize = new CGSize(1080, 1152); //1080, 3/5 * 1920
				byte[] data = new byte[(int)targetSize.Height * 4 * (int)targetSize.Width];

				resultBuffer = CVPixelBuffer.Create((nint)targetSize.Width, (nint)targetSize.Height, CVPixelFormatType.CV32BGRA, data, 4 * (nint)targetSize.Width, null); 

				if (resultBuffer == null) throw new Exception("Can't allocate pixel buffer.");
			}

			context.Render(cropped, resultBuffer);

			//  For debugging
			//var image = ImageBufferToUIImage(resultBuffer);
			//Console.WriteLine("Image size: " + image.Size); // set breakpoint to see image being provided to CoreML

			return resultBuffer;
		}

		private bool disposed = false;
		protected override void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			if (context != null)
			{
				context.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
