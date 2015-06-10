using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ViewAnimations
{
	public partial class ViewAnimationsViewController : UIViewController
	{
		UIImageView imageView;
		UIImage image;

		public ViewAnimationsViewController () : base ("ViewAnimationsViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			image = UIImage.FromFile ("monkeys.jpg");

			imageView = new UIImageView (new CGRect (new CGPoint (0, 0), image.Size)) {
				Image =  image
			};

			View.AddSubview (imageView);

			AnimateViewWithKeyframes ();
		}

		void AnimateViewWithKeyframes ()
		{
			var initialTransform = imageView.Transform;
			var initialCeneter = imageView.Center;

			// can now use keyframes directly on UIView without needing to drop directly into Core Animation

			UIView.AnimateKeyframes (2.0, 0, UIViewKeyframeAnimationOptions.Autoreverse, () => {
				UIView.AddKeyframeWithRelativeStartTime (0.0, 0.5, () => { 
					imageView.Center = new CGPoint (200, 200);
				});

				UIView.AddKeyframeWithRelativeStartTime (0.5, 0.5, () => { 
					imageView.Transform = CGAffineTransform.MakeRotation ((float)Math.PI / 2);
				});
			}, (finished) => {
				imageView.Center = initialCeneter;
				imageView.Transform = initialTransform;

				AnimateWithSpring ();
			});
		}

		void AnimateWithSpring ()
		{ 
			float springDampingRatio = 0.25f;
			float initialSpringVelocity = 1.0f;

			UIView.AnimateNotify (3.0, 0.0, springDampingRatio, initialSpringVelocity, 0, () => {

				imageView.Center = new CGPoint (imageView.Center.X, 400);	
					
			}, null);
		}
	}
}

