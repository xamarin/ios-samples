using System;
using UIKit;

using CoreGraphics;

namespace Example_Drawing.Screens.iPad.CoordinatesOnScreen
{
	public class View : UIView
	{
		#region -= constructors =-

		public View () : base()
		{
		}

		#endregion

		// rect changes depending on if the whole view is being redrawn, or just a section
		public override void Draw (CGRect rect)
		{
			Console.WriteLine ("Draw() Called");
			base.Draw (rect);
			
			// get a reference to the context
			using (CGContext context = UIGraphics.GetCurrentContext ()) {
				
				// declare vars
				int remainder;
				int textHeight = 20;
			
				// invert the 'y' coordinates on the text
				context.TextMatrix = CGAffineTransform.MakeScale (1, -1);
				
				#region -= vertical ticks =-
				
				// create our vertical tick lines
				using (CGLayer verticalTickLayer = CGLayer.Create (context, new CGSize (20, 3))) {
					// draw a single tick
					verticalTickLayer.Context.FillRect (new CGRect (0, 1, 20, 2));
					
					// draw a vertical tick every 20 pixels
					float yPos = 20;
					int numberOfVerticalTicks = (((int)Frame.Height / 20) - 1);
					for (int i = 0; i < numberOfVerticalTicks; i++) {
						
						// draw the layer
						context.DrawLayer (verticalTickLayer, new CGPoint (0, yPos));
						
						// starting at 40, draw the coordinate text nearly to the top
						if (yPos > 40 && i < (numberOfVerticalTicks - 2)) {
							
							// draw it every 80 points
							Math.DivRem ((int)yPos, (int)80, out remainder);
							if (remainder == 0)
								ShowTextAtPoint (context, 30, (yPos - (textHeight / 2)), yPos.ToString (), textHeight);
						}
						
						// increment the position of the next tick
						yPos += 20;
					}
				}
				
				#endregion
				
				#region -= horizontal ticks =-
				
				// create our horizontal tick lines
				using (CGLayer horizontalTickLayer = CGLayer.Create (context, new CGSize (3, 20))) {
					horizontalTickLayer.Context.FillRect (new CGRect (1, 0, 2, 20));
					
					// draw a horizontal tick every 20 pixels
					float xPos = 20;
					int numberOfHorizontalTicks = (((int)Frame.Width / 20) - 1);
					for (int i = 0; i < numberOfHorizontalTicks; i++) {
						context.DrawLayer (horizontalTickLayer, new CGPoint (xPos, 0));
						
						// starting at 100, draw the coordinate text nearly to the top
						if (xPos > 100 && i < (numberOfHorizontalTicks - 1)) {
							// draw it every 80 points
							Math.DivRem ((int)xPos, (int)80, out remainder);
							if (remainder == 0) {
								ShowCenteredTextAtPoint (context, xPos, 40, xPos.ToString (), textHeight);
							}
						}
						
						// increment the position of the next tick
						xPos += 20;
					}
				}
				
				#endregion
				
				// draw our "origin" text
				ShowTextAtPoint (context, 20, (30 + (textHeight / 2)), "Origin (0,0)", textHeight);
				
				#region -= points =-
				
				// (250,700)
				context.FillEllipseInRect (new CGRect (250, 700, 6, 6));
				ShowCenteredTextAtPoint (context, 250, 695, "(250,700)", textHeight);
				
				// (500,300)
				context.FillEllipseInRect (new CGRect (500, 300, 6, 6));
				ShowCenteredTextAtPoint (context, 500, 295, "(500,300)", textHeight);
				
				
				#endregion
			}
		}

		protected void ShowTextAtPoint (CGContext context, float x, float y, string text, int textHeight)
		{
			context.SelectFont ("Helvetica-Bold", textHeight, CGTextEncoding.MacRoman);
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			context.ShowTextAtPoint (x, y, text, text.Length);
		}

		private void ShowCenteredTextAtPoint (CGContext context, float centerX, float y, string text, int textHeight)
		{
			context.SelectFont ("Helvetica-Bold", textHeight, CGTextEncoding.MacRoman);
			context.SetTextDrawingMode (CGTextDrawingMode.Invisible);
			context.ShowTextAtPoint (centerX, y, text, text.Length);
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			context.ShowTextAtPoint (centerX - (context.TextPosition.X - centerX) / 2, y, text, text.Length);
		}
	}
}

