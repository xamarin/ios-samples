using System;
using UIKit;
using Accelerate;
using System.Diagnostics;
using CoreGraphics;

namespace UIImageEffects
{
	public static class UIImageEffects
	{
		public static UIImage ApplyLightEffect (this UIImage self)
		{
			var tintColor = UIColor.FromWhiteAlpha (1.0f, 0.3f);

			return ApplyBlur (self, blurRadius:30, tintColor:tintColor, saturationDeltaFactor:1.8f, maskImage:null);
		}

		public static UIImage ApplyExtraLightEffect (this UIImage self)
		{
			var tintColor = UIColor.FromWhiteAlpha (0.97f, 0.82f);

			return ApplyBlur (self, blurRadius:20, tintColor:tintColor, saturationDeltaFactor:1.8f, maskImage:null);
		}

		public static UIImage ApplyDarkEffect (this UIImage self)
		{
			var tintColor = UIColor.FromWhiteAlpha (0.11f, 0.73f);

			return ApplyBlur (self, blurRadius:20, tintColor:tintColor, saturationDeltaFactor:1.8f, maskImage:null);
		}

		public static UIImage ApplyTintEffect (this UIImage self, UIColor tintColor)
		{
			const float EffectColorAlpha = 0.6f;
			var effectColor = tintColor;
			nfloat alpha;
			var componentCount = tintColor.CGColor.NumberOfComponents;
			if (componentCount == 2) {
				nfloat white;
				if (tintColor.GetWhite (out white, out alpha))
					effectColor = UIColor.FromWhiteAlpha (white, EffectColorAlpha);
			} else {
				try {
					nfloat r, g, b;
					tintColor.GetRGBA (out r, out g, out b, out alpha);
					effectColor = UIColor.FromRGBA (r, g, b, EffectColorAlpha);
				} catch {
				}
			}
			return ApplyBlur (self, blurRadius: 10, tintColor: effectColor, saturationDeltaFactor: -1, maskImage: null);
		}

		public unsafe static UIImage ApplyBlur (UIImage image, float blurRadius, UIColor tintColor, float saturationDeltaFactor, UIImage maskImage)
		{
			if (image.Size.Width < 1 || image.Size.Height < 1) {
				Debug.WriteLine (@"*** error: invalid size: ({0} x {1}). Both dimensions must be >= 1: {2}", image.Size.Width, image.Size.Height, image);
				return null;
			}
			if (image.CGImage == null) {
				Debug.WriteLine (@"*** error: image must be backed by a CGImage: {0}", image);
				return null;
			}
			if (maskImage != null && maskImage.CGImage == null) {
				Debug.WriteLine (@"*** error: maskImage must be backed by a CGImage: {0}", maskImage);
				return null;
			}

			var imageRect = new CGRect (CGPoint.Empty, image.Size);
			var effectImage = image;

			bool hasBlur = blurRadius > float.Epsilon;
			bool hasSaturationChange = Math.Abs (saturationDeltaFactor - 1) > float.Epsilon;

			if (hasBlur || hasSaturationChange) {
				UIGraphics.BeginImageContextWithOptions (image.Size, false, UIScreen.MainScreen.Scale);
				var contextIn = UIGraphics.GetCurrentContext ();
				contextIn.ScaleCTM (1.0f, -1.0f);
				contextIn.TranslateCTM (0, -image.Size.Height);
				contextIn.DrawImage (imageRect, image.CGImage);
				var effectInContext = contextIn.AsBitmapContext () as CGBitmapContext;

				var effectInBuffer = new vImageBuffer () {
					Data = effectInContext.Data,
					Width = (int)effectInContext.Width,
					Height = (int)effectInContext.Height,
					BytesPerRow =(int) effectInContext.BytesPerRow
				};

				UIGraphics.BeginImageContextWithOptions (image.Size, false, UIScreen.MainScreen.Scale);
				var effectOutContext = UIGraphics.GetCurrentContext ().AsBitmapContext () as CGBitmapContext;
				var effectOutBuffer = new vImageBuffer () {
					Data = effectOutContext.Data,
					Width = (int)effectOutContext.Width,
					Height = (int)effectOutContext.Height,
					BytesPerRow = (int)effectOutContext.BytesPerRow
				};

				if (hasBlur) {
					var inputRadius = blurRadius * UIScreen.MainScreen.Scale;
					uint radius = (uint)(Math.Floor (inputRadius * 3 * Math.Sqrt (2 * Math.PI) / 4 + 0.5));
					if ((radius % 2) != 1)
						radius += 1;
					vImage.BoxConvolveARGB8888 (ref effectInBuffer, ref effectOutBuffer, IntPtr.Zero, 0, 0, radius, radius, Pixel8888.Zero, vImageFlags.EdgeExtend);
					vImage.BoxConvolveARGB8888 (ref effectOutBuffer, ref effectInBuffer, IntPtr.Zero, 0, 0, radius, radius, Pixel8888.Zero, vImageFlags.EdgeExtend);
					vImage.BoxConvolveARGB8888 (ref effectInBuffer, ref effectOutBuffer, IntPtr.Zero, 0, 0, radius, radius, Pixel8888.Zero, vImageFlags.EdgeExtend);
				}
				bool effectImageBuffersAreSwapped = false;
				if (hasSaturationChange) {
					var s = saturationDeltaFactor;
					var floatingPointSaturationMatrix = new float [] {
						0.0722f + 0.9278f * s,  0.0722f - 0.0722f * s,  0.0722f - 0.0722f * s,  0,
						0.7152f - 0.7152f * s,  0.7152f + 0.2848f * s,  0.7152f - 0.7152f * s,  0,
						0.2126f - 0.2126f * s,  0.2126f - 0.2126f * s,  0.2126f + 0.7873f * s,  0,
						0,                    0,                    0,  1,
					};
					const int divisor = 256;
					var saturationMatrix = new short [floatingPointSaturationMatrix.Length];
					for (int i = 0; i < saturationMatrix.Length; i++)
						saturationMatrix [i] = (short)Math.Round (floatingPointSaturationMatrix [i] * divisor);
					if (hasBlur) {
						vImage.MatrixMultiplyARGB8888 (ref effectOutBuffer, ref effectInBuffer, saturationMatrix, divisor, null, null, vImageFlags.NoFlags);
						effectImageBuffersAreSwapped = true;
					} else
						vImage.MatrixMultiplyARGB8888 (ref effectInBuffer, ref effectOutBuffer, saturationMatrix, divisor, null, null, vImageFlags.NoFlags);
				}
				if (!effectImageBuffersAreSwapped)
					effectImage = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();
				if (effectImageBuffersAreSwapped)
					effectImage = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();
			}

			// Setup up output context
			UIGraphics.BeginImageContextWithOptions (image.Size, false, UIScreen.MainScreen.Scale);
			var outputContext = UIGraphics.GetCurrentContext ();
			outputContext.ScaleCTM (1, -1);
			outputContext.TranslateCTM (0, -image.Size.Height);

			// Draw base image
			if (hasBlur) {
				outputContext.SaveState ();
				if (maskImage != null)
					outputContext.ClipToMask (imageRect, maskImage.CGImage);
				outputContext.DrawImage (imageRect, effectImage.CGImage);
				outputContext.RestoreState ();
			}

			if (tintColor != null) {
				outputContext.SaveState ();
				outputContext.SetFillColor (tintColor.CGColor);
				outputContext.FillRect (imageRect);
				outputContext.RestoreState ();
			}
			var outputImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return outputImage;
		}
	}
}

