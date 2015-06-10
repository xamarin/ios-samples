using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace UIKitDynamicsDemo
{
	public partial class UIKitDynamicsDemoViewController : UIViewController
	{
		UIImageView imageView;
		UIImage image;
		UIDynamicAnimator dynAnimator;

		public UIKitDynamicsDemoViewController ()
		{
			View.BackgroundColor = UIColor.White;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			using (image = UIImage.FromFile ("monkeys.jpg")) {

				imageView = new UIImageView (new CGRect (new CGPoint (View.Center.X - image.Size.Width / 2, 0), image.Size)) {
					Image =  image
				};

				View.AddSubview (imageView);

				// 1. create the dynamic animator
				dynAnimator = new UIDynamicAnimator (this.View);

				// 2. create behavior(s)
				var dynItems = new IUIDynamicItem[] { imageView };
				var gravity = new UIGravityBehavior (dynItems);
				var collision = new UICollisionBehavior (dynItems) {
					TranslatesReferenceBoundsIntoBoundary = true
				};
				var dynBehavior = new UIDynamicItemBehavior (dynItems) {
					Elasticity = 0.7f
				};

				// 3. add behaviors(s) to the dynamic animator
				dynAnimator.AddBehavior (gravity);
				dynAnimator.AddBehavior (collision);
				dynAnimator.AddBehavior (dynBehavior);
			}
		}
	}
}