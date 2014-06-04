using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace LineLayout
{
	public partial class Cell : UICollectionViewCell
	{
		public UILabel Label { get; private set; }
		
		[Export("initWithFrame:")]
		public Cell (CGRect frame) : base (frame)
		{   
			Label = new UILabel (new CGRect (CGPoint.Empty, frame.Size)) {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight|UIViewAutoresizing.FlexibleWidth,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.BoldSystemFontOfSize (50f),
				BackgroundColor = UIColor.UnderPageBackgroundColor,
				TextColor = UIColor.Black
			};

			ContentView.AddSubview (Label);
			ContentView.Layer.BorderWidth = 1.0f;
			ContentView.Layer.BorderColor = UIColor.White.CGColor;
		}
	}
}