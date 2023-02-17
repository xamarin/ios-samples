using UIKit;
using CoreGraphics;

namespace CustomTransitions {
	public class SwipeTransitionAnimator : UIViewControllerAnimatedTransitioning {

		readonly UIRectEdge targetEdge;

		public SwipeTransitionAnimator (UIRectEdge edge)
		{
			targetEdge = edge;
		}

		public override double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return 0.35;
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			var fromViewController = transitionContext.GetViewControllerForKey (UITransitionContext.FromViewControllerKey);
			var toViewController = transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);

			UIView containerView = transitionContext.ContainerView;
			var fromView = transitionContext.GetViewFor (UITransitionContext.FromViewKey);
			var toView = transitionContext.GetViewFor (UITransitionContext.ToViewKey);

			bool isPresenting = (toViewController.PresentingViewController == fromViewController);

			var fromFrame = transitionContext.GetInitialFrameForViewController (fromViewController);
			var toFrame = transitionContext.GetFinalFrameForViewController (toViewController);

			var offset = new CGVector (0f, 0f);

			switch (targetEdge) {
			case UIRectEdge.Top:
				offset = new CGVector (0f, 1f);
				break;
			case UIRectEdge.Bottom:
				offset = new CGVector (0f, -1f);
				break;
			case UIRectEdge.Left:
				offset = new CGVector (1f, 0f);
				break;
			case UIRectEdge.Right:
				offset = new CGVector (-1, 0);
				break;
			default:
				offset = new CGVector (0f, 0f);
				// TODO: error
				break;
			}

			if (isPresenting) {
				fromView.Frame = fromFrame;
				toView.Frame = new CGRect (toFrame.X + (toFrame.Size.Width * offset.dx * -1), toFrame.Y + (toFrame.Size.Height * offset.dy * -1), toFrame.Size.Width, toFrame.Size.Height);
			} else {
				fromView.Frame = fromFrame;
				toView.Frame = toFrame;
			}

			if (isPresenting) {
				containerView.AddSubview (toView);
			} else {
				containerView.InsertSubviewBelow (toView, fromView);
			}

			var duration = TransitionDuration (transitionContext);

			UIView.Animate (duration, 0, UIViewAnimationOptions.TransitionNone, () => {
				toView.Frame = isPresenting ?
					toFrame : new CGRect (fromFrame.X + (fromFrame.Size.Width * offset.dx), fromFrame.Y + (fromFrame.Size.Height * offset.dy), fromFrame.Size.Width, fromFrame.Size.Height);
			}, () => {
				bool wasCancel = transitionContext.TransitionWasCancelled;

				if (wasCancel) {
					toView.RemoveFromSuperview ();
					transitionContext.CompleteTransition (!wasCancel);
				}
			});
		}
	}
}
