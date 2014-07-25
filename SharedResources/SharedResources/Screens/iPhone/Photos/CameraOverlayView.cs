using System;
using UIKit;
using CoreGraphics;


namespace Example_SharedResources.Screens.iPhone.Photos
{
	public class CameraOverlayView : UIView
	{
		#region -= constructors =-

		public CameraOverlayView() : base() { Initialize(); }
		public CameraOverlayView (CGRect frame) : base(frame) { Initialize(); }
		
		protected void Initialize()
		{
			this.BackgroundColor = UIColor.Clear;
		}

		#endregion
		
		// rect changes depending on if the whole view is being redrawn, or just a section
		public override void Draw (CGRect rect)
		{
			Console.WriteLine ("Draw() Called");
			base.Draw (rect);
			
			// get a reference to the context
			using (CGContext context = UIGraphics.GetCurrentContext ()) {
				// convert to View space
				CGAffineTransform affineTransform = CGAffineTransform.MakeIdentity ();
				// invert the y axis
				affineTransform.Scale (1, -1);
				// move the y axis up
				affineTransform.Translate (0, Frame.Height);
				context.ConcatCTM (affineTransform);
				
				// draw some stars
				DrawStars (context);
			}
		}
		
		protected void DrawStars (CGContext context)
		{
			context.SetFillColor (1f, 0f, 0f, 1f);

			// save state so that as we translate (move the origin around, 
			// it goes back to normal when we restore)
			context.SetFillColor (0f, 0f, 0.329f, 1.0f);
			context.SaveState ();
			context.TranslateCTM (30, 300);
			DrawStar (context, 30);
			context.RestoreState ();

			context.SetFillColor (1f, 0f, 0f, 1f);
			context.SaveState ();
			context.TranslateCTM (120, 200);
			DrawStar (context, 30);
			context.RestoreState ();

		}
		
		/// <summary>
		/// Draws a star at the bottom left of the context of the specified diameter
		/// </summary>
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
			//context.SetRGBFillColor (1, 1, 1, 1);
			context.ClosePath ();
			context.FillPath ();
		}

	}
}

