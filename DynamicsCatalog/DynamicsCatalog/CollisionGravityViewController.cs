using System;
using UIKit;

namespace DynamicsCatalog {

	public partial class CollisionGravityViewController : UIViewController {

		public UIDynamicAnimator Animator { get; private set; }

		public CollisionGravityViewController (IntPtr handle) : base (handle)
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

			// Another style of creating the UIDynamicAnimator
			Animator = new UIDynamicAnimator (View) { gravityBehavior, collisionBehavior };
		}
	}
}