using System;
using UIKit;

namespace DynamicsCatalog {

	public partial class ItemPropertiesViewController : UIViewController {

		public UIDynamicAnimator Animator { get; private set; }

		public ItemPropertiesViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			/*
     			We want to show collisions between views and boundaries with different 
     			elasticities, we thus associate the two views to gravity and collision 
     			behaviors. We will only change the restitution parameter for one of 
     			these views.
     		*/
			var gravityBehavior = new UIGravityBehavior (square1, square2);

			var collisionBehavior = new UICollisionBehavior (square1, square2) {
				TranslatesReferenceBoundsIntoBoundary = true
			};
			collisionBehavior.BeganBoundaryContact += (sender, e) => {
				((UIView)e.DynamicItem).BackgroundColor = UIColor.LightGray;
			};
			collisionBehavior.EndedBoundaryContact += (sender, e) => {
				((UIView)e.DynamicItem).BackgroundColor = UIColor.Gray;
			};

			var propertiesBehavior = new UIDynamicItemBehavior (square2) {
				Elasticity = 0.5f
			};
	
			Animator = new UIDynamicAnimator (View);
			Animator.AddBehaviors (gravityBehavior, collisionBehavior, propertiesBehavior);
		}
	}
}