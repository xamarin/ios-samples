using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreGraphics;

namespace ManualCameraControls
{
	/// <summary>
	/// Helper class that pulls an image from the sample buffer and displays it in the <c>UIImageView</c>
	/// that it has been attached to.
	/// </summary>
	public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the display view.
		/// </summary>
		/// <value>The display view.</value>
		public UIImageView DisplayView { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ManualCameraControls.OutputRecorder"/> class.
		/// </summary>
		public OutputRecorder ()
		{

		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Gets a single image frame from sample buffer.
		/// </summary>
		/// <returns>The image from sample buffer.</returns>
		/// <param name="sampleBuffer">Sample buffer.</param>
		private UIImage GetImageFromSampleBuffer(CMSampleBuffer sampleBuffer) {

			// Get a pixel buffer from the sample buffer
			using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
				// Lock the base address
				pixelBuffer.Lock (0);

				// Prepare to decode buffer
				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;

				// Decode buffer - Create a new colorspace
				using (var cs = CGColorSpace.CreateDeviceRGB ()) {

					// Create new context from buffer
					using (var context = new CGBitmapContext (pixelBuffer.BaseAddress,
						                     pixelBuffer.Width,
						                     pixelBuffer.Height,
						                     8,
						                     pixelBuffer.BytesPerRow,
						                     cs,
						                     (CGImageAlphaInfo)flags)) {

						// Get the image from the context
						using (var cgImage = context.ToImage ()) {

							// Unlock and return image
							pixelBuffer.Unlock (0);
							return UIImage.FromImage (cgImage);
						}
					}
				}
			}
		}
		#endregion

		#region Override Methods
		/// <Docs>The capture output on which the frame was captured.</Docs>
		/// <param name="connection">The connection on which the video frame was received.</param>
		/// <remarks>Unless you need to keep the buffer for longer, you must call
		///  Dispose() on the sampleBuffer before returning. The system
		///  has a limited pool of video frames, and once it runs out of
		///  those buffers, the system will stop calling this method
		///  until the buffers are released.</remarks>
		/// <summary>
		/// Dids the output sample buffer.
		/// </summary>
		/// <param name="captureOutput">Capture output.</param>
		/// <param name="sampleBuffer">Sample buffer.</param>
		public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			// Trap all errors
			try {
				// Grab an image from the buffer
				var image = GetImageFromSampleBuffer(sampleBuffer);

				// Display the image
				if (DisplayView !=null) {
					DisplayView.BeginInvokeOnMainThread(() => {
						// Set the image
						var oldImg = DisplayView.Image;
						oldImg?.Dispose ();

						DisplayView.Image = image;

						// Rotate image to the correct display orientation
						DisplayView.Transform = CGAffineTransform.MakeRotation((float)Math.PI/2);
					});
				}

				// IMPORTANT: You must release the buffer because AVFoundation has a fixed number
				// of buffers and will stop delivering frames if it runs out.
				sampleBuffer.Dispose();
			}
			catch(Exception e) {
				// Report error
				Console.WriteLine ("Error sampling buffer: {0}", e.Message);
			}
		}
		#endregion
	}
}

