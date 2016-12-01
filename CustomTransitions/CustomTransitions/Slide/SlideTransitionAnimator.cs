using UIKit;
using System;
using CoreGraphics;

namespace CustomTransitions
{
	public class SlideTransitionAnimator : UIViewControllerAnimatedTransitioning
	{
		public UIRectEdge targetEdge;

		public SlideTransitionAnimator(UIRectEdge edge)
		{
			targetEdge = edge;
		}

		public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
		{ 
			UIViewController fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
			UIViewController toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

			UIView containerView = transitionContext.ContainerView;

			// In iOS 8, the viewForKey: method was introduced to get views that the
			// animator manipulates.  This method should be preferred over accessing
			// the view of the fromViewController/toViewController directly.
			// It may return nil whenever the animator should not touch the view
			// (based on the presentation style of the incoming view controller).
			// It may also return a different view for the animator to animate.
			UIView fromView = transitionContext.GetViewFor(UITransitionContext.FromViewKey);
			UIView toView = transitionContext.GetViewFor(UITransitionContext.ToViewKey);

			bool isPresenting = (toViewController.PresentingViewController == fromViewController);

			CGRect fromFrame = transitionContext.GetInitialFrameForViewController(fromViewController);
			CGRect toFrame = transitionContext.GetFinalFrameForViewController(toViewController);

			CGVector offset = new CGVector(0, 0);
				if (this.targetEdge == UIRectEdge.Left)
			{
				offset = new CGVector(-1, 0);
			}
			else if (this.targetEdge == UIRectEdge.Right) {
				offset = new CGVector(1, 0);
			}

			fromView.Frame = fromFrame;

			CGRect auxFrame = toFrame;
			auxFrame.Offset(toFrame.Width * offset.dx * -1, toFrame.Height * offset.dy * -1);

			toView.Frame = auxFrame;

			containerView.AddSubview(toView);

			double duration = this.TransitionDuration(transitionContext);

			UIView.Animate(duration, 0, UIViewAnimationOptions.TransitionNone,
				() =>
				{
					CGRect fromFrameAux = fromFrame;
					fromFrameAux.Offset(fromFrame.Width * offset.dx, fromFrame.Height * offset.dy);
					fromView.Frame = fromFrameAux;

					toView.Frame = toFrame;
				},
				() =>
				{
					bool wasCancel = transitionContext.TransitionWasCancelled;
					transitionContext.CompleteTransition(!wasCancel);
				}
			);


		}

		public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
		{
			return 0.35;
		}
	}
}
