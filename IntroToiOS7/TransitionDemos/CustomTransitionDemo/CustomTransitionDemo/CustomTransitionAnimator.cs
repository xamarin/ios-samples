using System;
using CoreGraphics;
using UIKit;

namespace CustomTransitionDemo
{
	public class CustomTransitionAnimator : UIViewControllerAnimatedTransitioning
	{
		public CustomTransitionAnimator ()
		{
		}

		public override double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return 1.0;
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			var inView = transitionContext.ContainerView;
			var toVC = transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);
			var toView = toVC.View;

			inView.AddSubview (toView);

			var frame = toView.Frame;
			toView.Frame = CGRect.Empty;

			UIView.Animate (TransitionDuration (transitionContext), () => {
				toView.Frame = new CGRect (20, 20, frame.Width - 40, frame.Height - 40);
			}, () => {
				transitionContext.CompleteTransition (true);
			});
		}
	}

	public class TransitioningDelegate : UIViewControllerTransitioningDelegate
	{
		CustomTransitionAnimator animator;

		public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source)
		{
			animator = new CustomTransitionAnimator ();
			return animator;
		}
	}
}

