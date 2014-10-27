using System;
using System.Threading;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Metal;
using OpenTK;
using UIKit;

namespace MetalTexturedQuad
{
	public class Texture : NSObject
	{
		string path;
		bool flip;

		public nint Height { get; private set; }

		public nint Width  { get; private set; }

		public IMTLTexture MetalTexture { get; private set; }

		public Texture (string name, string extension)
		{
			path = NSBundle.MainBundle.PathForResource (name, extension);
			Width = 0;
			Height = 0;
			MetalTexture = null;
			flip = true;
		}

		public bool Finalize (IMTLDevice device)
		{
			if (MetalTexture != null)
				return true;

			UIImage image = UIImage.FromFile (path);

			if (image == null)
				return false;

			using (CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ()) {
				if (colorSpace == null)
					return false;

				Width = image.CGImage.Width;
				Height = image.CGImage.Height;

				nuint width = (nuint)Width;
				nuint height = (nuint)Height;
				nuint rowBytes = width * 4;

				var context = new CGBitmapContext (IntPtr.Zero,
					              (int)width,
					              (int)height,
					              8,
					              (int)rowBytes,
					              colorSpace,
					              CGImageAlphaInfo.PremultipliedLast);

				if (context == null)
					return false;

				var bounds = new CGRect (0f, 0f, width, height);
				context.ClearRect (bounds);

				// Vertical Reflect
				if (flip) {
					context.TranslateCTM (width, height);
					context.ScaleCTM (-1f, -1f);
				}

				context.DrawImage (bounds, image.CGImage);
				MTLTextureDescriptor texDesc = MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA8Unorm, width, height, false);

				if (texDesc == null)
					return false;

				MetalTexture = device.CreateTexture (texDesc);

				if (MetalTexture == null) {
					context.Dispose ();
					return false;
				}

				IntPtr pixels = context.Data;

				if (pixels != IntPtr.Zero) {
					var region = new MTLRegion ();
					region.Origin.X = 0;
					region.Origin.Y = 0;
					region.Size.Width = (nint)width;
					region.Size.Height = (nint)height;

					MetalTexture.ReplaceRegion (region, 0, pixels, rowBytes);
				}

				context.Dispose ();
			}

			return true;
		}
	}
}
