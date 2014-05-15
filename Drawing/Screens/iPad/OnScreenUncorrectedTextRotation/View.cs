using System;
using UIKit;

using CoreGraphics;

namespace Example_Drawing.Screens.iPad.OnScreenUncorrectedTextRotation
{
	public class View : UIView
	{
		#region -= constructors =-

		public View () : base() { }

		#endregion

		// rect changes depending on if the whole view is being redrawn, or just a section
		public override void Draw (CGRect rect)
		{
			Console.WriteLine ("Draw() Called");
			base.Draw (rect);
			
			// get a reference to the context
			using (CGContext context = UIGraphics.GetCurrentContext ()) {
				
				// declare vars
				ShowCenteredTextAtPoint (context, 384, 400, "Hello World!", 60);
			}
		}

		protected void ShowCenteredTextAtPoint (CGContext context, float centerX, float y, string text, int textHeight)
		{
			context.SelectFont ("Helvetica-Bold", textHeight, CGTextEncoding.MacRoman);
			context.SetTextDrawingMode (CGTextDrawingMode.Invisible);
			context.ShowTextAtPoint (centerX, y, text, text.Length);
			context.SetTextDrawingMode (CGTextDrawingMode.Fill);
			context.ShowTextAtPoint (centerX - (context.TextPosition.X - centerX) / 2, y, text, text.Length);
		}
	}
}

