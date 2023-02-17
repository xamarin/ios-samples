using System;
using CoreGraphics;
using CoreVideo;
using UIKit;

namespace CoreMLImageRecognition {
	public static class UIImageExtensions {
		public static CVPixelBuffer ToCVPixelBuffer (this UIImage self)
		{
			var attrs = new CVPixelBufferAttributes ();
			attrs.CGImageCompatibility = true;
			attrs.CGBitmapContextCompatibility = true;

			var cgImg = self.CGImage;

			var pb = new CVPixelBuffer (cgImg.Width, cgImg.Height, CVPixelFormatType.CV32ARGB, attrs);
			pb.Lock (CVPixelBufferLock.None);
			var pData = pb.BaseAddress;
			var colorSpace = CGColorSpace.CreateDeviceRGB ();
			var ctxt = new CGBitmapContext (pData, cgImg.Width, cgImg.Height, 8, pb.BytesPerRow, colorSpace, CGImageAlphaInfo.NoneSkipFirst);
			ctxt.TranslateCTM (0, cgImg.Height);
			ctxt.ScaleCTM (1.0f, -1.0f);
			UIGraphics.PushContext (ctxt);
			self.Draw (new CGRect (0, 0, cgImg.Width, cgImg.Height));
			UIGraphics.PopContext ();
			pb.Unlock (CVPixelBufferLock.None);

			return pb;

		}
	}
}
