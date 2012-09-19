using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace LineLayout
{
	public partial class Cell : UICollectionViewCell
	{
		public UILabel Label { get; private set; }
		
		[Export("initWithFrame:")]
		public Cell (RectangleF frame) : base (frame)
		{   
			Label = new UILabel (new RectangleF (PointF.Empty, frame.Size)) {
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