using System;
using UIKit;
using CoreGraphics;


namespace Example_Drawing.Screens.iPad.Shadows
{
	public class Controller : UIViewController
	{
		UIImageView imageView;

		#region -= constructors =-

		public Controller () : base() { }

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// set the background color of the view to white
			View.BackgroundColor = UIColor.White;
			
			// instantiate a new image view that takes up the whole screen and add it to 
			// the view hierarchy
			CGRect imageViewFrame = new CGRect (0, -NavigationController.NavigationBar.Frame.Height, View.Frame.Width, View.Frame.Height);
			imageView = new UIImageView (imageViewFrame);
			View.AddSubview (imageView);
			
			// create our offscreen bitmap context
			// size
			CGSize bitmapSize = new CGSize (View.Frame.Size);
			using (CGBitmapContext context = new CGBitmapContext (IntPtr.Zero, (int)bitmapSize.Width, (int)bitmapSize.Height, 8, (int)(4 * bitmapSize.Width), CGColorSpace.CreateDeviceRGB (), CGImageAlphaInfo.PremultipliedFirst)) {

				//==== create a grayscale shadow
				// 1) save graphics state
				context.SaveState ();
				// 2) set shadow context for offset and blur
				context.SetShadow (new CGSize (10, -10), 15);
				// 3) perform your drawing operation
				context.SetFillColor (.3f, .3f, .9f, 1);
				context.FillRect (new CGRect (100, 600, 300, 250));
				// 4) restore the graphics state
				context.RestoreState ();
				
				//==== create a color shadow
				// 1) save graphics state
				context.SaveState ();
				// 2) set shadow context for offset and blur
				context.SetShadow (new CGSize (15, -15), 10, UIColor.Blue.CGColor);				// 3) perform your drawing operation
				context.SelectFont ("Helvetica-Bold", 40, CGTextEncoding.MacRoman);
				context.SetTextDrawingMode (CGTextDrawingMode.Fill);
				string text = "Shadows are fun and easy!";
				context.ShowTextAtPoint (150, 200, text, text.Length);
				// 4) restore the graphics state
				context.RestoreState ();
				
				
				// output the drawing to the view
				imageView.Image = UIImage.FromImage (context.ToImage ());
			}
		}
	}
}
