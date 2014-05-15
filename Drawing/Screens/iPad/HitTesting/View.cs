using System;
using UIKit;

using CoreGraphics;
using Foundation;

namespace Example_Drawing.Screens.iPad.HitTesting
{
	public class View : UIView
	{
		CGPath myRectangleButtonPath;
		bool touchStartedInPath;
		
		#region -= constructors =-
		
		public View () : base() { }
	
		#endregion
	
		// rect changes depending on if the whole view is being redrawn, or just a section
		public override void Draw (CGRect rect)
		{
			Console.WriteLine ("Draw() Called");
			base.Draw (rect);
			
			using (CGContext context = UIGraphics.GetCurrentContext ()) {
				// draw a rectangle using a path
				myRectangleButtonPath = new CGPath ();
				myRectangleButtonPath.AddRect (new CGRect (new CGPoint (100, 10), new CGSize (200, 400)));
				context.AddPath (myRectangleButtonPath);
				context.DrawPath (CGPathDrawingMode.Stroke);
			}
		}
		
		// Raised when a user begins a touch on the screen. We check to see if the touch 
		// was within our path. If it was, we set the _touchStartedInPath = true so that 
		// we can track to see if when the user raised their finger, it was also in the path
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			// get a reference to the touch
			UITouch touch = touches.AnyObject as UITouch;
			// make sure there was one
			if (touch != null) {
				// check to see if the location of the touch was within our path
				if (myRectangleButtonPath.ContainsPoint (touch.LocationInView (this), true))
					touchStartedInPath = true;
			}
		}
		
		// Raised when a user raises their finger from the screen. Since we need to check to 
		// see if the user touch started and ended within the path, we have to track to see
		// when the finger is raised, if it did.
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			
			// get a reference to any of the touches
			UITouch touch = touches.AnyObject as UITouch;
			
			// if there is a touch
			if (touch != null) {
				
				// the point of touch
				CGPoint pt = touch.LocationInView (this);
				
				// if the touch ended in the path AND it started in the path
				if (myRectangleButtonPath.ContainsPoint (pt, true) && touchStartedInPath) {
					Console.WriteLine ("touched at location: " + pt.ToString ());
					UIAlertView alert = new UIAlertView ("Hit!", "You sunk my battleship!", null, "OK", null);
					alert.Show ();
				}
			}
			
			// reset
			touchStartedInPath = false;
		}

		// if for some reason the touch was cancelled, we clear our _touchStartedInPath flag
		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
			touchStartedInPath = false;
		}
	
	}
}