using System;
using UIKit;
using Foundation;
using CoreGraphics;

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

			View.BackgroundColor = UIColor.White;

			drawing = new DrawingView ();
			nfloat statusBarHeight = UIDevice.CurrentDevice.CheckSystemVersion (7,0) ?
				UIApplication.SharedApplication.StatusBarFrame.Height : 0f;
			drawing.Frame = new CoreGraphics.CGRect (0, statusBarHeight, 320, 640 - statusBarHeight);
			
			View.AddSubview (drawing);
			
			
			text = new UITextView (new CGRect (10, 150 + statusBarHeight, 300, 300 - statusBarHeight));
			text.Font = UIFont.SystemFontOfSize (14f);
			text.Editable = false;
			text.Text = "Xamarin Shapes Example\n\n"+
				"These are a few random shapes drawn with PaintCode and rendered in a UIView\n\n"
				+"http://www.paintcodeapp.com/";
			View.AddSubview (text);
		}
	}
}

