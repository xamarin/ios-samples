using System;

using UIKit;
using CoreGraphics;
using Foundation;

namespace Chat
{
	public class CellHelper
	{
		public static UIImage CreateColoredImage(UIColor color, UIImage mask)
		{
			var rect = new CGRect (CGPoint.Empty, mask.Size);
			UIGraphics.BeginImageContextWithOptions (mask.Size, false, mask.CurrentScale);
			CGContext context = UIGraphics.GetCurrentContext ();
			mask.DrawAsPatternInRect (rect);
			context.SetFillColor (color.CGColor);
			context.SetBlendMode (CGBlendMode.SourceAtop);
			context.FillRect (rect);
			UIImage result = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return result;
		}

		public static nfloat CellHeightFor(Message msg)
		{
			var attributes = new UIStringAttributes {
				Font = UIFont.SystemFontOfSize(17)
			};
			CGRect size =  ((NSString)msg.Text).GetBoundingRect (new CGSize (220, 0), NSStringDrawingOptions.UsesLineFragmentOrigin, attributes, null);
			return NMath.Ceiling(size.Height) + 24;
		}
	}
}

