using System;
using UIKit;
using CoreGraphics;


namespace Example_Drawing.Screens.iPad.StencilPattern
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

				// declare vars
				CGRect patternRect = new CGRect (0, 0, 16, 16);
				
				// set the color space of our fill to be the patter colorspace
				context.SetFillColorSpace (CGColorSpace.CreatePattern (CGColorSpace.CreateDeviceRGB()));
				
				// create a new pattern
				CGPattern pattern = new CGPattern (patternRect, CGAffineTransform.MakeRotation (.3f)
					, 16, 16, CGPatternTiling.NoDistortion, false, DrawPolkaDotPattern);
					
				// set our fill as our pattern, color doesn't matter because the pattern handles it
				context.SetFillPattern (pattern, new nfloat[] { 1, 0, 0, 1 });
				
				// fill the entire view with that pattern
				context.FillRect (imageView.Frame);
				
				// output the drawing to the view
				imageView.Image = UIImage.FromImage (context.ToImage ());
			}
		}

		/// <summary>
		/// This is our pattern callback. it's called by coregraphics to create 
		/// the pattern base.
		/// </summary>
		protected void DrawPolkaDotPattern (CGContext context)
		{
			context.FillEllipseInRect (new CGRect (4, 4, 8, 8));
		}
		
		/// <summary>
		/// This is a slightly more complicated draw pattern, but using it is just 
		/// as easy as the previous one. To use this one, simply change "DrawPolkaDotPattern"
		/// in line 54 above to "DrawStarPattern"
		/// </summary>
		protected void DrawStarPattern (CGContext context)
		{
			// declare vars
			float starDiameter = 16;
			// 144º
			float theta = 2 * (float)Math.PI * (2f / 5f);
			float radius = starDiameter / 2;
			
			// move up and over
			context.TranslateCTM (starDiameter / 2, starDiameter / 2);
			
			context.MoveTo (0, radius);
			for (int i = 1; i < 5; i++) {
				context.AddLineToPoint (radius * (float)Math.Sin (i * theta), radius * (float)Math.Cos (i * theta));
			}
			// fill our star as dark gray
			context.ClosePath ();
			context.FillPath ();
		}
		
	}
}
