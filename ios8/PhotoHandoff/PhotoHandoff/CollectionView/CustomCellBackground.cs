using System;

using UIKit;
using Foundation;

namespace PhotoHandoff
{
	public class CustomCellBackground : UIView
	{
		public CustomCellBackground ()
		{
			BackgroundColor = UIColor.LightGray;
			Layer.CornerRadius = 5;
		}
	}
}