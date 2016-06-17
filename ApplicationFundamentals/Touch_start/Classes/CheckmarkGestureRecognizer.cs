using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace Touch
{
	public class CheckmarkGestureRecognizer : UIGestureRecognizer
	{
		#region Private Variables
		private CGPoint midpoint = CGPoint.Empty;
		private bool strokeUp = false;
		#endregion 

		#region Override Methods
		/// <summary>
		///   Called when the touches end or the recognizer state fails
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			strokeUp = false;
			midpoint = CGPoint.Empty;
		}

		/// <summary>
		///   Is called when the fingers touch the screen.
		/// </summary>
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			// we want one and only one finger
			if (touches.Count != 1)
			{
				base.State = UIGestureRecognizerState.Failed;
			}

			Console.WriteLine(base.State.ToString());
		}

		/// <summary>
		///   Called when the touches are cancelled due to a phone call, etc.
		/// </summary>
		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
			// we fail the recognizer so that there isn't unexpected behavior 
			// if the application comes back into view
			base.State = UIGestureRecognizerState.Failed;
		}

		/// <summary>
		///   Called when the fingers lift off the screen
		/// </summary>
		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			//
			if (base.State == UIGestureRecognizerState.Possible && strokeUp)
			{
				base.State = UIGestureRecognizerState.Recognized;
			}

			Console.WriteLine(base.State.ToString());
		}

		/// <summary>
		///   Called when the fingers move
		/// </summary>
		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			// if we haven't already failed
			if (base.State != UIGestureRecognizerState.Failed)
			{
				// get the current and previous touch point
				CGPoint newPoint = (touches.AnyObject as UITouch).LocationInView(View);
				CGPoint previousPoint = (touches.AnyObject as UITouch).PreviousLocationInView(View);

				// if we're not already on the upstroke
				if (!strokeUp)
				{
					// if we're moving down, just continue to set the midpoint at 
					// whatever point we're at. when we start to stroke up, it'll stick
					// as the last point before we upticked
					if (newPoint.X >= previousPoint.X && newPoint.Y >= previousPoint.Y)
					{
						midpoint = newPoint;
					}
					// if we're stroking up (moving right x and up y [y axis is flipped])
					else if (newPoint.X >= previousPoint.X && newPoint.Y <= previousPoint.Y)
					{
						strokeUp = true;
					}
					// otherwise, we fail the recognizer
					else
					{
						base.State = UIGestureRecognizerState.Failed;
					}
				}
			}

			Console.WriteLine(base.State.ToString());
		}
		#endregion
	}
}

