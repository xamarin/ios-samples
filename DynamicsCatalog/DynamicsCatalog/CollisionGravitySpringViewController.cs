using System;
using CoreGraphics;
using UIKit;

namespace DynamicsCatalog {

	public partial class CollisionGravitySpringViewController : UIViewController {

		public UIDynamicAnimator Animator { get; private set; }

		public CollisionGravitySpringViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var gravityBehavior = new UIGravityBehavior (square);

			var collisionBehavior = new UICollisionBehavior (square) {
				TranslatesReferenceBoundsIntoBoundary = true
			};
			collisionBehavior.BeganBoundaryContact += (sender, e) => {
				((UIView)e.DynamicItem).BackgroundColor = UIColor.LightGray;
			};
			collisionBehavior.EndedBoundaryContact += (sender, e) => {
				((UIView)e.DynamicItem).BackgroundColor = UIColor.Gray;
			};

			var squareCenterPoint = new CGPoint (square.Center.X, square.Center.Y - 100);

			var attachmentBehavior = new UIAttachmentBehavior (square, squareCenterPoint) {
				Frequency = 1.0f,
				Damping = 0.1f
			};
		
			redSquare.Center = attachmentBehavior.AnchorPoint;
			blueSquare.Center = new CGPoint (50.0f, 50.0f);

			Animator = new UIDynamicAnimator (View);
			Animator.AddBehaviors (attachmentBehavior, gravityBehavior, collisionBehavior);

			View.AddGestureRecognizer (new UIPanGestureRecognizer ((gesture) => {
				attachmentBehavior.AnchorPoint = gesture.LocationInView (View);
				redSquare.Center = attachmentBehavior.AnchorPoint;
			}));
		}
	}
}