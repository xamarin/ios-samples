using System;
using UIKit;
using CoreGraphics;


namespace Example_Drawing.Screens.iPad.FlagOffScreen
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
			CGSize bitmapSize = new CGSize (imageView.Frame.Size);
			using (CGBitmapContext context = new CGBitmapContext (IntPtr.Zero,
									      (int)bitmapSize.Width, (int)bitmapSize.Height, 8,
									      (int)(4 * bitmapSize.Width), CGColorSpace.CreateDeviceRGB (),
									      CGImageAlphaInfo.PremultipliedFirst)) {
				
				// draw our coordinates for reference
				DrawCoordinateSpace (context);
				
				// draw our flag
				DrawFlag (context);
				
				// add a label
				DrawCenteredTextAtPoint (context, 384, 700, "Stars and Stripes", 60);
								
				// output the drawing to the view
				imageView.Image = UIImage.FromImage (context.ToImage ());
			}
		}
		
		protected void DrawFlag (CGContext context)
		{
			// declare vars
			int i, j;
			
			// general sizes
			float flagWidth = (float) imageView.Frame.Width * .8f;
			float flagHeight = (float)(flagWidth / 1.9);
			CGPoint flagOrigin = new CGPoint (imageView.Frame.Width * .1f, imageView.Frame.Height / 3);
			
			// stripe
			float stripeHeight = flagHeight / 13;
			float stripeSpacing = stripeHeight * 2;
			CGRect stripeRect = new CGRect (0, 0, flagWidth, stripeHeight);
			
			// star field
			float starFieldHeight = 7 * stripeHeight;
			float starFieldWidth = flagWidth * (2f / 5f);
			CGRect starField = new CGRect (flagOrigin.X, flagOrigin.Y + (6 * stripeHeight), starFieldWidth, starFieldHeight);
			
			// stars
			float starDiameter = flagHeight * 0.0616f;
			float starHorizontalCenterSpacing = (starFieldWidth / 6);
			float starHorizontalPadding = (starHorizontalCenterSpacing / 4);
			float starVerticalCenterSpacing = (starFieldHeight / 5);
			float starVerticalPadding = (starVerticalCenterSpacing / 4);
			CGPoint firstStarOrigin = new CGPoint (flagOrigin.X + starHorizontalPadding, flagOrigin.Y + flagHeight - starVerticalPadding - (starVerticalCenterSpacing / 2));
			CGPoint secondRowFirstStarOrigin = new CGPoint (firstStarOrigin.X + (starHorizontalCenterSpacing / 2), firstStarOrigin.Y - (starVerticalCenterSpacing / 2));
			
			// white background + shadow
			context.SaveState ();
			context.SetShadow (new CGSize (15, -15), 7);
			context.SetFillColor (1, 1, 1, 1);
			context.FillRect (new CGRect (flagOrigin.X, flagOrigin.Y, flagWidth, flagHeight));
			context.RestoreState ();
			
			// create a stripe layer
			using (CGLayer stripeLayer = CGLayer.Create (context, stripeRect.Size)) {
				
				// set red as the fill color
				// this works
				stripeLayer.Context.SetFillColor (1f, 0f, 0f, 1f);
				// but this doesn't ????
				//stripeLayer.Context.SetFillColor (new float[] { 1f, 0f, 0f, 1f });
				// fill the stripe
				stripeLayer.Context.FillRect (stripeRect);
				
				// loop through the stripes and draw the layer
				context.SaveState ();
				for (i = 0; i < 7; i++) {
					Console.WriteLine ("drawing stripe layer");
					// draw the layer
					context.DrawLayer (stripeLayer, flagOrigin);
					// move the origin
					context.TranslateCTM (0, stripeSpacing);
				}
				context.RestoreState ();
			}
			
			// draw the star field
			//BUGBUG: apple bug - this only works on on-screen CGContext and CGLayer
			//context.SetFillColor (new float[] { 0f, 0f, 0.329f, 1.0f });
			context.SetFillColor (0f, 0f, 0.329f, 1.0f);
			context.FillRect (starField);
			
			// create the star layer
			using (CGLayer starLayer = CGLayer.Create (context, starField.Size)) {
				
				// draw the stars
				DrawStar (starLayer.Context, starDiameter);
				
				// 6-star rows
				// save state so that as we translate (move the origin around, 
				// it goes back to normal when we restore)
				context.SaveState ();
				context.TranslateCTM (firstStarOrigin.X, firstStarOrigin.Y);
				// loop through each row
				for (j = 0; j < 5; j++) {
					
					// each star in the row
					for (i = 0; i < 6; i++) {
						
						// draw the star, then move the origin to the right
						context.DrawLayer (starLayer, new CGPoint (0f, 0f));
						context.TranslateCTM (starHorizontalCenterSpacing, 0f);
					}
					// move the row down, and then back left
					context.TranslateCTM ( (-i * starHorizontalCenterSpacing), -starVerticalCenterSpacing);
				}
				context.RestoreState ();
				
				// 5-star rows			
				context.SaveState ();
				context.TranslateCTM (secondRowFirstStarOrigin.X, secondRowFirstStarOrigin.Y);
				// loop through each row
				for (j = 0; j < 4; j++) {
					
					// each star in the row
					for (i = 0; i < 5; i++) {
						
						context.DrawLayer (starLayer, new CGPoint (0f, 0f));
						context.TranslateCTM (starHorizontalCenterSpacing, 0);
					}
					context.TranslateCTM ( (-i * starHorizontalCenterSpacing), -starVerticalCenterSpacing);
				}
				context.RestoreState ();
			}
		}
		
		// Draws the specified text starting at x,y of the specified height to the context.
		protected void DrawTextAtPoint (CGContext context, float x, float y, string text, int textHeight)
		{
			// configure font
			context.SelectFont ("Helvetica-Bold", textHeight, CGTextEncoding.MacRoman);
			// set it to fill in our text, don't just outline
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			// call showTextAtPoint
			context.ShowTextAtPoint (x, y, text, text.Length);
		}
		
		protected void DrawCenteredTextAtPoint (CGContext context, float centerX, float y, string text, int textHeight)
		{
			context.SelectFont ("Helvetica-Bold", textHeight, CGTextEncoding.MacRoman);
			context.SetTextDrawingMode (CGTextDrawingMode.Invisible);
			context.ShowTextAtPoint (centerX, y, text, text.Length);
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			context.ShowTextAtPoint (centerX - (context.TextPosition.X - centerX) / 2, y, text, text.Length);
		}
		
		// Draws a star at the bottom left of the context of the specified diameter
		protected void DrawStar (CGContext context, float starDiameter)
		{
			// declare vars
			// 144º
			float theta = 2 * (float)Math.PI * (2f / 5f);
			float radius = starDiameter / 2;

			// move up and over
			context.TranslateCTM (starDiameter / 2, starDiameter / 2);
			
			context.MoveTo (0, radius);
			for (int i = 1; i < 5; i++) {
				context.AddLineToPoint (radius * (float)Math.Sin (i * theta), radius * (float)Math.Cos (i * theta));
			}
			context.SetFillColor (1, 1, 1, 1);
			context.ClosePath ();
			context.FillPath ();
		}

		
		// Draws our coordinate grid
		protected void DrawCoordinateSpace (CGBitmapContext context)
		{
			// declare vars
			int remainder;
			int textHeight = 20;
			
			#region -= vertical ticks =-
			
			// create our vertical tick lines
			using (CGLayer verticalTickLayer = CGLayer.Create (context, new CGSize (20, 3)))
			{
				// draw a single tick
				verticalTickLayer.Context.FillRect (new CGRect (0, 1, 20, 2));
				
				// draw a vertical tick every 20 pixels
				float yPos = 20;
				int numberOfVerticalTicks = ((context.Height / 20) - 1);
				for (int i = 0; i < numberOfVerticalTicks; i++) {
					
					// draw the layer
					context.DrawLayer (verticalTickLayer, new CGPoint (0, yPos));
					
					// starting at 40, draw the coordinate text nearly to the top
					if (yPos > 40 && i < (numberOfVerticalTicks - 2)) {
						
						// draw it every 80 points
						Math.DivRem ((int)yPos, (int)80, out remainder);
						if (remainder == 0)
							DrawTextAtPoint (context, 30, (yPos - (textHeight / 2)), yPos.ToString (), textHeight);
					}
					
					// increment the position of the next tick
					yPos += 20;
				}
			}
			
			#endregion
			
			#region -= horizontal ticks =-
			
			// create our horizontal tick lines
			using (CGLayer horizontalTickLayer = CGLayer.Create (context, new CGSize (3, 20)))
			{
				horizontalTickLayer.Context.FillRect (new CGRect (1, 0, 2, 20));
				
				// draw a horizontal tick every 20 pixels
				float xPos = 20;
				int numberOfHorizontalTicks = ((context.Width / 20) - 1);
				for (int i = 0; i < numberOfHorizontalTicks; i++) {
					
					context.DrawLayer (horizontalTickLayer, new CGPoint (xPos, 0));
					
					// starting at 100, draw the coordinate text nearly to the top
					if (xPos > 100 && i < (numberOfHorizontalTicks - 1)) {
						
						// draw it every 80 points
						Math.DivRem ((int)xPos, (int)80, out remainder);
						if (remainder == 0)
							DrawCenteredTextAtPoint (context, xPos, 30, xPos.ToString (), textHeight);
					}
					
					// increment the position of the next tick
					xPos += 20;
				}
			}
			
			#endregion
			
			// draw our "origin" text
			DrawTextAtPoint (context, 20, (20 + (textHeight / 2)), "Origin (0,0)", textHeight);
			
		}
	}
}

