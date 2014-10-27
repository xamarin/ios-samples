using System;

using UIKit;
using CoreGraphics;

namespace LookInside
{
	public class OverlayAnimatedTransitioning : UIViewControllerAnimatedTransitioning
	{
		public bool IsPresentation { get; set; }

		public OverlayAnimatedTransitioning ()
		{
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

			UIViewController animatingVC = IsPresentation ? toVC : fromVC;
			UIView animatingView = animatingVC.View;

			CGRect appearedFrame = transitionContext.GetFinalFrameForViewController (animatingVC);
			CGRect dismissedFrame = appearedFrame;

			CGPoint targetLacation = dismissedFrame.Location;
			targetLacation.X += dismissedFrame.Size.Width;
			dismissedFrame.Location = targetLacation;

			CGRect initialFrame = IsPresentation ? dismissedFrame : appearedFrame;
			CGRect finalFrame = IsPresentation ? appearedFrame : dismissedFrame;

			animatingView.Frame = initialFrame;

			UIViewAnimationOptions opt = UIViewAnimationOptions.AllowUserInteraction
				                       | UIViewAnimationOptions.BeginFromCurrentState;
			await UIView.AnimateNotifyAsync (TransitionDuration (transitionContext), 0, 300, 5, opt, () => {
				animatingView.Frame = finalFrame;
			});

			if (!IsPresentation)
				fromView.RemoveFromSuperview ();

			transitionContext.CompleteTransition (true);
		}

		public override double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return 0.5;
		}
	}
}

