using System;
using OpenTK.Graphics.ES20;
using OpenGLES;
using System.IO;
using Foundation;
using UIKit;
using CoreImage;
using CoreGraphics;

namespace OpenGLES20Example {
	public class GLTexture {
		string filename;
		uint texture;

		public GLTexture (string inFilename)
		{
			GL.Enable (EnableCap.Texture2D);
			GL.Enable (EnableCap.Blend);

			filename = inFilename;

			GL.Hint (HintTarget.GenerateMipmapHint, HintMode.Nicest);
			GL.GenTextures (1, out texture);
			GL.BindTexture (TextureTarget.Texture2D, texture);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) All.Repeat);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) All.Repeat);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Linear);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Linear);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);

			//TODO Remove the Substring method if you don't support iOS versions prior to iOS 6.
			string extension = Path.GetExtension (filename).Substring (1);
			string baseFilename = Path.GetFileNameWithoutExtension (filename);

			string path = NSBundle.MainBundle.PathForResource (baseFilename, extension);
			NSData texData = NSData.FromFile (path);

			UIImage image = UIImage.LoadFromData (texData);
			if (image == null)
				return;

			nint width = image.CGImage.Width;
			nint height = image.CGImage.Height;

			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ();
			byte [] imageData = new byte [height * width * 4];
			CGContext context = new CGBitmapContext (imageData, width, height, 8, 4 * width, colorSpace,
													  CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);

			context.TranslateCTM (0, height);
			context.ScaleCTM (1, -1);
			colorSpace.Dispose ();
			context.ClearRect (new CGRect (0, 0, width, height));
			context.DrawImage (new CGRect (0, 0, width, height), image.CGImage);

			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int) width, (int) height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageData);
			context.Dispose ();
		}

		public static void UseDefaultTexture ()
		{
			GL.BindTexture (TextureTarget.Texture2D, 0);
		}

		public void Use ()
		{
			GL.BindTexture (TextureTarget.Texture2D, texture);
		}
	}
}

