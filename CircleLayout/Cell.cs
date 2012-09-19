using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace CircleLayout
{
	public class Cell : UICollectionViewCell
	{
		[Export ("initWithFrame:")]
		public Cell (RectangleF frame) : base (frame)
		{
			ContentView.Layer.CornerRadius = 35.0f;
			ContentView.Layer.BorderWidth = 1.0f;
			ContentView.Layer.BorderColor = UIColor.White.CGColor;
			ContentView.BackgroundColor = UIColor.UnderPageBackgroundColor;
		}
	}
}