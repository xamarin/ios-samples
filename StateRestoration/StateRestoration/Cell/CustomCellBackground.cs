using System;
using CoreGraphics;

using UIKit;

namespace StateRestoration
{
	public class CustomCellBackground : UIView
	{
		CustomCellBackground (CGRect frame) : base (frame)
		{
			BackgroundColor = UIColor.White;
			Layer.CornerRadius = 5;
		}

		public static UIView CreateCustomCellBackground (CGRect frame)
		{
			return new CustomCellBackground (frame);
		}
	}
}
