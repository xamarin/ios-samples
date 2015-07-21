using System;

using Foundation;
using UIKit;

namespace SegueCatalog
{
	public partial class CustomAnimationPresentationSegue : UIStoryboardSegue, IUIViewControllerTransitioningDelegate, IUIViewControllerAnimatedTransitioning
	{
		public CustomAnimationPresentationSegue (IntPtr handle) : base (handle)
		{
		}

		public override void Perform ()
		{
			/*
			Because this class is used for a Present Modally segue, UIKit will 
			maintain a strong reference to this segue object for the duration of
			the presentation. That way, this segue object will still be around to
			provide an animation controller for the eventual dismissal, as well 
			as for the initial presentation.
			*/
			DestinationViewController.TransitioningDelegate = this;
			base.Perform ();
		}

		[Export ("animationControllerForPresentedController:presentingController:sourceController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source)
		{
			return this;
		}

		[Export ("animationControllerForDismissedController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController (UIViewController dismissed)
		{
			return this;
		}

		public double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return 0.5;
		}

		public void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			UIView containerView = transitionContext.ContainerView;
			UIView toView = transitionContext.GetViewFor (UITransitionContext.ToViewKey);

			if (transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey) == DestinationViewController) {
				// Presenting.
				UIView.PerformWithoutAnimation (() => {
					toView.Alpha = 0;
					containerView.AddSubview (toView);
				});

				double transitionContextDuration = TransitionDuration (transitionContext);

				UIView.AnimateNotify (transitionContextDuration, () => {
					toView.Alpha = 1;
				}, transitionContext.CompleteTransition);
			} else {
				// Dismissing.
				var fromView = transitionContext.GetViewFor (UITransitionContext.FromViewKey);
				UIView.PerformWithoutAnimation (() => containerView.InsertSubviewBelow (toView, fromView));

				var transitionContextDuration = TransitionDuration (transitionContext);

				UIView.AnimateNotify (transitionContextDuration, () => {
					fromView.Alpha = 0;
				}, transitionContext.CompleteTransition);
			}
		}
	}
}
