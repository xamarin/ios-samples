using UIKit;
using CoreGraphics;

namespace CustomTransitions {
	public class SlideTransitionAnimator : UIViewControllerAnimatedTransitioning {
		UIRectEdge targetEdge;

		public SlideTransitionAnimator (UIRectEdge edge)
		{
			targetEdge = edge;
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			var fromViewController = transitionContext.GetViewControllerForKey (UITransitionContext.FromViewControllerKey);
			var toViewController = transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);

			UIView containerView = transitionContext.ContainerView;

			var fromView = transitionContext.GetViewFor (UITransitionContext.FromViewKey);
			var toView = transitionContext.GetViewFor (UITransitionContext.ToViewKey);

			var fromFrame = transitionContext.GetInitialFrameForViewController (fromViewController);
			var toFrame = transitionContext.GetFinalFrameForViewController (toViewController);

			var offset = new CGVector (0f, 0f);

			if (targetEdge == UIRectEdge.Left)
				offset = new CGVector (-1f, 0f);
			else if (targetEdge == UIRectEdge.Right)
				offset = new CGVector (1f, 0f);

			fromView.Frame = fromFrame;

			CGRect auxFrame = toFrame;
			auxFrame.Offset (toFrame.Width * offset.dx * -1f, toFrame.Height * offset.dy * -1f);
			toView.Frame = auxFrame;

			containerView.AddSubview (toView);

			var duration = TransitionDuration (transitionContext);

			UIView.Animate (duration, 0, UIViewAnimationOptions.TransitionNone, () => {
				var fromFrameAux = fromFrame;
				fromFrameAux.Offset (fromFrame.Width * offset.dx, fromFrame.Height * offset.dy);
				fromView.Frame = fromFrameAux;

				toView.Frame = toFrame;
			}, () => {
				transitionContext.CompleteTransition (!transitionContext.TransitionWasCancelled);
			}
			);
		}

		public override double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return 0.35;
		}
	}
}
