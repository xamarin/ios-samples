using System;
using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using ImageIO;
using CoreVideo;
using System.Diagnostics;

namespace BracketStripes
{
	public class StripedImage : NSObject
	{
		private CGSize imageSize;
		private CGSize stripeSize;
		private int stride;
		private int stripeIndex;
		private CGBitmapContext renderContext;

		public StripedImage (CGSize size, int stripWidth, int stride)
		{
			imageSize = size;
			this.stride = stride;
			stripeSize = new CGSize (stripWidth, size.Height);

			PrepareImage (size);
		}

		private void PrepareImage (CGSize size)
		{
			nint bitsPerComponent = 8;
			nint width = (nint)size.Width;
			nint paddedWidth = (width + 15);
			nint bytesPerPixel = 4;
			nint bytesPerRow = paddedWidth * bytesPerPixel;

			using (var colorSpace = CGColorSpace.CreateDeviceRGB ()) {
				renderContext = new CGBitmapContext (null, (nint)size.Width, (nint)size.Height, bitsPerComponent, bytesPerRow, colorSpace, CGImageAlphaInfo.PremultipliedFirst);
			}
		}

		public void AddSampleBuffer (CMSampleBuffer sampleBuffer)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			using (var image = CreateImage (sampleBuffer)) {
				var imageRect = new CGRect (0, 0, image.Width, image.Height);

				var maskRects = new List<CGRect> ();
				var maskRect = new CGRect (stripeSize.Width * stripeIndex, 0, stripeSize.Width, stripeSize.Height);

				while (maskRect.X < imageSize.Width) {
					maskRects.Add (maskRect);
					maskRect.X += stripeSize.Width * stride;
				}

				renderContext.SaveState ();
				renderContext.ClipToRects (maskRects.ToArray ());
				renderContext.DrawImage (imageRect, image);
				renderContext.RestoreState ();
			}

			stopwatch.Stop();
			Console.WriteLine ("Render time for contributor {0}: {1} msec", stripeIndex, stopwatch.Elapsed);

			stripeIndex = (stripeIndex + 1) % stride;
		}

		public UIImage ImageWithOrientation (UIImageOrientation orientation)
		{
			nfloat scale = UIScreen.MainScreen.Scale;
			UIImage image = null;

			using (CGImage cgImage = renderContext.ToImage ()) {
				image = UIImage.FromImage (cgImage, scale, orientation);
			}

			return image;
		}

		protected override void Dispose (bool disposing)
		{
			renderContext.Dispose ();
			base.Dispose (disposing);
		}

		private CGImage CreateImage (CMSampleBuffer sampleBuffer)
		{
			CGImage image = null;

			CMVideoFormatDescription formatDescription = sampleBuffer.GetVideoFormatDescription ();
			var subType = formatDescription.MediaSubType;
			CMBlockBuffer blockBuffer = sampleBuffer.GetDataBuffer ();

			if (blockBuffer != null) {
				if (subType != (int)CMVideoCodecType.JPEG)
					throw new Exception ("Block buffer must be JPEG encoded.");

				var jpegData = new NSMutableData ();
				jpegData.Length = blockBuffer.DataLength;

				blockBuffer.CopyDataBytes (0, blockBuffer.DataLength, jpegData.Bytes);

				using (var imageSource = CGImageSource.FromData (jpegData)) {
					var decodeOptions = new CGImageOptions {
						ShouldAllowFloat = false,
						ShouldCache = false
					};

					image = imageSource.CreateImage (0, decodeOptions);
				}
			} else {

				if (subType != (int)CVPixelFormatType.CV32BGRA)
					throw new Exception ("Image buffer must be BGRA encoded.");

				CVImageBuffer imageBuffer = sampleBuffer.GetImageBuffer ();

				using (var colorSpace = CGColorSpace.CreateDeviceRGB ())
				using (var bitmapContext = new CGBitmapContext (imageBuffer.Handle,
					                           (int)imageBuffer.DisplaySize.Width, (int)imageBuffer.DisplaySize.Height, 8, 0, colorSpace, CGImageAlphaInfo.NoneSkipFirst)) {
					image = bitmapContext.ToImage ();
				}
			}

			return image;
		}
	}
}
