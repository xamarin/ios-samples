using System;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace TransitionsDemo.AnimationControllers
{
	public class CEExplosionAnimationController : CEReversibleAnimationController
	{
		public CEExplosionAnimationController ()
		{
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext,
		                                        UIViewController fromViewController, UIViewController toViewController,
		                                        UIView fromView, UIView toView)
		{
			UIView containerView = transitionContext.ContainerView;
			containerView.AddSubview (toView);
			containerView.SendSubviewToBack (toView);

			var size = toView.Frame.Size;
			var snapshots = new List<UIView> ();

			float xFactor = 10f;
			nfloat yFactor = xFactor * size.Height / size.Width;

			for (nfloat x = 0; x < size.Width; x += size.Width / xFactor) {
				for (nfloat y = 0; y < size.Height; y += size.Height / yFactor) {
					var snapshotRegion = new CGRect (x, y, size.Width / xFactor, size.Height / yFactor);
					UIView snapshot = fromView.ResizableSnapshotView (snapshotRegion, false, UIEdgeInsets.Zero);

					snapshot.Frame = snapshotRegion;
					containerView.AddSubview (snapshot);
					snapshots.Add (snapshot);
				}
			}

			containerView.SendSubviewToBack (fromView);

			var rnd = new Random ();
			double duration = TransitionDuration (transitionContext);
			Action action = () => {
				foreach (UIView view in snapshots) {
					float xOffset = rnd.Next (-100, 100);
					float yOffset = rnd.Next (-100, 100);
					view.Frame = new CGRect (view.Frame.X + xOffset, view.Frame.Y + yOffset, view.Frame.Width, view.Frame.Height);
					view.Alpha = 0f;
					var transition = CGAffineTransform.MakeRotation (rnd.Next (-10, 10));
					transition.Scale (0f, 0f);
					view.Transform = transition;
				}
			};

			UIView.Animate (duration, action, () => {
				foreach (UIView view in snapshots)
					view.RemoveFromSuperview ();

				transitionContext.CompleteTransition (!transitionContext.TransitionWasCancelled);
			});
		}
	}
}

