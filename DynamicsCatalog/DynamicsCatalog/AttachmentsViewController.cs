using System;
using CoreGraphics;
using UIKit;

namespace DynamicsCatalog {

	public partial class AttachmentsViewController : UIViewController {

		public UIDynamicAnimator Animator { get; private set; }

		public AttachmentsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var collisionBehavior = new UICollisionBehavior (square);
			collisionBehavior.TranslatesReferenceBoundsIntoBoundary = true;

			var squareCenterPoint = new CGPoint (square.Center.X, square.Center.Y - 100);
			var attachmentOffset = new UIOffset (-25.0f, -25.0f);

			/*
    		 By default, an attachment behavior uses the center of a view. By using a small offset, 
    		 we get a more interesting effect which will cause the view to have rotation movement 
    		 when dragging the attachment.
    		*/
			var attachmentBehavior = new UIAttachmentBehavior (square, attachmentOffset, squareCenterPoint);

			// Show visually the attachment points
			redSquare.Center = attachmentBehavior.AnchorPoint;
			blueSquare.Center = new CGPoint (25.0f, 25.0f);

			Animator = new UIDynamicAnimator (View);
			Animator.AddBehavior (attachmentBehavior);

			View.AddGestureRecognizer (new UIPanGestureRecognizer ((gesture) => {
				attachmentBehavior.AnchorPoint = gesture.LocationInView (View);
				redSquare.Center = attachmentBehavior.AnchorPoint;
			}));
		}
	}
}