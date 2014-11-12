using System;
using UIKit;
using CoreGraphics;
using CoreGraphics;

namespace Example_Drawing.Screens.iPad.DrawOffScreenUsingCGBitmapContext
{
	public class Controller : UIViewController
	{
		#region -= constructors =-

		public Controller () : base()
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// no data
			IntPtr data = IntPtr.Zero;
			// size
			CGSize bitmapSize = new CGSize (200, 300);
			//View.Frame.Size;
			// 32bit RGB (8bits * 4components (aRGB) = 32bit)
			int bitsPerComponent = 8;
			// 4bytes for each pixel (32 bits = 4bytes)
			int bytesPerRow = (int)(4 * bitmapSize.Width);
			// no special color space
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ();
			// aRGB
			CGImageAlphaInfo alphaType = CGImageAlphaInfo.PremultipliedFirst;
			
			
			using (CGBitmapContext context = new CGBitmapContext (data
				, (int)bitmapSize.Width, (int)bitmapSize.Height, bitsPerComponent
				, bytesPerRow, colorSpace, alphaType)) {
				
				// draw whatever here.
			}
			
			
			
		}
	}
}

