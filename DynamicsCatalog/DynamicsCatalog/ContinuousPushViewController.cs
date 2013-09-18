using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace DynamicsCatalog {

	public partial class ContinuousPushViewController : UIViewController {

		public UIDynamicAnimator Animator { get; private set; }

		public ContinuousPushViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var collisionBehavior = new UICollisionBehavior (square) {
				TranslatesReferenceBoundsIntoBoundary = true
			};

			var pushBehavior = new UIPushBehavior (UIPushBehaviorMode.Continuous, square) {
				Angle = 0.0f,
				Magnitude = 0.0f
			};

			Animator = new UIDynamicAnimator (View);
			Animator.AddBehaviors (collisionBehavior, pushBehavior);

			redSquare.Center = new PointF (View.Bounds.GetMidX (), View.Bounds.GetMidY ());
			redSquare.Layer.AnchorPoint = new PointF (0.0f, 0.5f);

			View.AddGestureRecognizer (new UITapGestureRecognizer ((gesture) => {
				/*
	    		 Tapping will change the angle and magnitude of the impulse. 
	    		 To visually show the impulse vector on screen, a red line representing 
	    		 the angle and magnitude of this vector is briefly drawn.
	    		 */
				PointF p = gesture.LocationInView (View);
				PointF o = new PointF (View.Bounds.GetMidX (), View.Bounds.GetMidY ());
				float distance = (float) Math.Sqrt ((p.X - o.X) * (p.X - o.X) + (p.Y - o.Y) * (p.Y - o.Y));
				float angle = (float) Math.Atan2 (p.Y - o.Y, p.X - o.X);
				distance = Math.Min (distance, 200.0f);

				redSquare.Bounds = new RectangleF (0.0f, 0.0f, distance, 5.0f);
				redSquare.Transform = CGAffineTransform.MakeRotation (angle);

				pushBehavior.Magnitude = distance / 100.0f;
				pushBehavior.Angle = angle;
			}));
		}
	}
}