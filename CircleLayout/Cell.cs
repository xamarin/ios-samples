using UIKit;
using Foundation;
using CoreGraphics;

namespace CircleLayout
{
	public class Cell : UICollectionViewCell
	{
		[Export ("initWithFrame:")]
		public Cell (CGRect frame) : base (frame)
		{
			ContentView.Layer.CornerRadius = 35.0f;
			ContentView.Layer.BorderWidth = 1.0f;
			ContentView.Layer.BorderColor = UIColor.White.CGColor;
			ContentView.BackgroundColor = UIColor.LightGray;
		}
	}
}