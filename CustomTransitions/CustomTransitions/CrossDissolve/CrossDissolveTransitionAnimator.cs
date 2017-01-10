using Foundation;
using UIKit;

namespace CustomTransitions
{
	public class CrossDissolveTransitionAnimator : UIViewControllerAnimatedTransitioning
	{

		public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
		{
			return 0.35;
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

			fromView.Frame = transitionContext.GetInitialFrameForViewController(fromViewController);
			toView.Frame = transitionContext.GetFinalFrameForViewController(toViewController);

			fromView.Alpha = 1;
			toView.Alpha = 0;

			containerView.AddSubview(toView);

			double transitionDuration = this.TransitionDuration(transitionContext);

			UIView.Animate(transitionDuration, 0, UIViewAnimationOptions.TransitionNone,
				() =>
				{
					fromView.Alpha = 0;
					toView.Alpha = 1;
				},
				() =>
				{
					bool wasCancel = transitionContext.TransitionWasCancelled;
					transitionContext.CompleteTransition(!wasCancel);
				}
			);
		}
	}
}
