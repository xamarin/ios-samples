using System;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;

namespace PinchIt
{
	public partial class Cell : UICollectionViewCell
	{
		UILabel label;

		[Export ("initWithFrame:")]
		public Cell (RectangleF frame) : base (frame)	
		{
			label = new UILabel (new RectangleF (PointF.Empty, frame.Size)) {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.BoldSystemFontOfSize (50.0f),
				BackgroundColor = UIColor.Clear,

			};

			UIImageView imageView = new UIImageView (new UIImage ("Images/rupert.png")) {
				Frame = label.Frame
			};

			label.Add (imageView);
			ContentView.AddSubview (label);

			ContentView.Layer.BorderWidth = 1.0f;
			ContentView.Layer.BorderColor = UIColor.White.CGColor;
		}
	}
}

