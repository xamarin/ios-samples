using System;
using System.Drawing;

using MonoTouch.UIKit;

namespace StateRestoration
{
	public class CustomCellBackground : UIView
	{
		CustomCellBackground (RectangleF frame) : base (frame)
		{
			BackgroundColor = UIColor.White;
			Layer.CornerRadius = 5;
		}

		public static UIView CreateCustomCellBackground (RectangleF frame)
		{
			return new CustomCellBackground (frame);
		}
	}
}
