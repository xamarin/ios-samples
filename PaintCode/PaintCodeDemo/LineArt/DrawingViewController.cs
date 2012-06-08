using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace PaintCode
{
	public class DrawingViewController : UIViewController
	{
		public DrawingViewController ()
		{
		}

		UIView drawing;
		UITextView text;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			drawing = new DrawingView ();
			drawing.Frame = new System.Drawing.RectangleF (0, 0, 320, 640);
			
			View.AddSubview (drawing);
			
			
			text = new UITextView (new Rectangle (10, 150, 300, 300));
			text.Font = UIFont.SystemFontOfSize (14f);
			text.Editable = false;
			text.Text = "Xamarin Shapes Example\n\n"+
				"These are a few random shapes drawn with PaintCode and rendered in a UIView\n\n"
				+"http://www.paintcodeapp.com/";
			View.AddSubview (text);
		}
	}
}

