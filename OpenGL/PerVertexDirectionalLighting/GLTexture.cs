using System;
using OpenTK.Graphics.ES20;
using OpenGLES;
using System.IO;
using Foundation;
using UIKit;
using CoreImage;

using CoreGraphics;

namespace PerVertexDirectionalLighting
{
	public class GLTexture
	{
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

			string extension = Path.GetExtension (filename);
			string baseFilename = Path.GetFileNameWithoutExtension (filename);

			string path = NSBundle.MainBundle.PathForResource (baseFilename, extension);
			NSData texData = NSData.FromFile (path);

			UIImage image = UIImage.LoadFromData (texData);
			if (image == null)
				return;

			int width = (int)image.CGImage.Width;
			int height = (int)image.CGImage.Height;

			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ();
			byte [] imageData = new byte[height * width * 4];
			CGContext context = new CGBitmapContext  (imageData, width, height, 8, 4 * width, colorSpace,
			                                          CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);

			context.TranslateCTM (0, height);
			context.ScaleCTM (1, -1);
			colorSpace.Dispose ();
			context.ClearRect (new CGRect (0, 0, width, height));
			context.DrawImage (new CGRect (0, 0, width, height), image.CGImage);

			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageData);
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

