using System;
using UIKit;
using Foundation;

namespace TransitionsDemo.AnimationControllers
{
	public class CEReversibleAnimationController : UIViewControllerAnimatedTransitioning
	{
		public bool Reverse { get; set; }

		public float Duration { get; private set; }

		public CEReversibleAnimationController ()
		{
			Duration = 1.0f;
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			UIViewController fromViewController = transitionContext.GetViewControllerForKey (UITransitionContext.FromViewControllerKey);
			UIViewController toViewController = transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);

			UIView fromView = fromViewController.View;
			UIView toView = toViewController.View;

			AnimateTransition (transitionContext, fromViewController, toViewController, fromView, toView);
		}

		public override double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return Duration;
		}

		public virtual void AnimateTransition (IUIViewControllerContextTransitioning transitionContext, 
		                                      UIViewController fromViewController, UIViewController toViewController, 
		                                      UIView fromView, UIView toView)
		{
		}
	}
}

