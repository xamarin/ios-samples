using System;
using CoreGraphics;

using UIKit;

using Foundation;

namespace PinchIt
{
	public partial class Cell : UICollectionViewCell
	{
		UILabel label;

		[Export ("initWithFrame:")]
		public Cell (CGRect frame) : base (frame)	
		{
			label = new UILabel (new CGRect (CGPoint.Empty, frame.Size)) {
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

