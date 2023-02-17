using UIKit;

namespace CustomTransitions {
	public class CrossDissolveTransitionAnimator : UIViewControllerAnimatedTransitioning {

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

			fromView.Frame = transitionContext.GetInitialFrameForViewController (fromViewController);
			toView.Frame = transitionContext.GetFinalFrameForViewController (toViewController);

			fromView.Alpha = 1f;
			toView.Alpha = 0f;

			containerView.AddSubview (toView);
			var transitionDuration = TransitionDuration (transitionContext);

			UIView.Animate (transitionDuration, 0, UIViewAnimationOptions.TransitionNone, () => {
				fromView.Alpha = 0f;
				toView.Alpha = 1f;
			}, () => {
				bool wasCancel = transitionContext.TransitionWasCancelled;
				transitionContext.CompleteTransition (!wasCancel);
			}
			);
		}
	}
}
