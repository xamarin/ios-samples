using System;

using UIKit;
using CoreGraphics;

namespace LookInside
{
	public class CoolAnimatedTransitioning : UIViewControllerAnimatedTransitioning
	{
		public bool IsPresentation { get; set; }

		public CoolAnimatedTransitioning ()
		{
		}

		public override double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return 0.5;
		}

		public async override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			UIViewController fromVC = transitionContext.GetViewControllerForKey (UITransitionContext.FromViewControllerKey);
			UIView fromView = fromVC.View;
			UIViewController toVC = transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);
			UIView toView = toVC.View;

			UIView containerView = transitionContext.ContainerView;
			if (IsPresentation)
				containerView.AddSubview (toView);

			UIViewController animatingVC = IsPresentation? toVC : fromVC;
			UIView animatingView = animatingVC.View;

			animatingView.Frame = transitionContext.GetFinalFrameForViewController (animatingVC);

			CGAffineTransform presentedTransform = CGAffineTransform.MakeIdentity ();
			CGAffineTransform dismissedTransform = CGAffineTransform.MakeScale (0.001f, 0.001f) * CGAffineTransform.MakeRotation (8 * (float)Math.PI);

			animatingView.Transform = IsPresentation ? dismissedTransform : presentedTransform;

			var opt = UIViewAnimationOptions.AllowUserInteraction | UIViewAnimationOptions.BeginFromCurrentState;
			await UIView.AnimateNotifyAsync (TransitionDuration (transitionContext), 0, 300, 5, opt, () => {
				animatingView.Transform = IsPresentation ? presentedTransform : dismissedTransform;
			});
			animatingView.Transform = CGAffineTransform.MakeIdentity ();

			if (!IsPresentation)
				fromView.RemoveFromSuperview ();

			transitionContext.CompleteTransition (true);
		}
	}
}

