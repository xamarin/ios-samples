using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace TransitionsDemo.AnimationControllers
{
	public class CECrossfadeAnimationController : CEReversibleAnimationController
	{
		public CECrossfadeAnimationController ()
		{
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext, 
		                                        UIViewController fromViewController, UIViewController toViewController, 
		                                        UIView fromView, UIView toView)
		{
			UIView containerView = transitionContext.ContainerView;
			containerView.AddSubview (toView);
			containerView.SendSubviewToBack (toView);

			double duration = TransitionDuration (transitionContext);
			NSAction animation = () => {
				fromView.Alpha = 0f;
			};

			UIView.Animate (duration, animation, () => {
				if (transitionContext.TransitionWasCancelled) {
					fromView.Alpha = 1f;
				} else {
					fromView.RemoveFromSuperview ();
					fromView.Alpha = 1f;
				}

				transitionContext.CompleteTransition (!transitionContext.TransitionWasCancelled);
			});
		}
	}
}

