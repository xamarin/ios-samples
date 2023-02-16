using UIKit;
using Foundation;

namespace CustomTransitions {
	public class SwipeTransitionDelegate : NSObject, IUIViewControllerTransitioningDelegate {

		public UIScreenEdgePanGestureRecognizer GestureRecognizer { get; set; }

		public UIRectEdge TargetEdge { get; set; }

		[Export ("animationControllerForPresentedController:presentingController:sourceController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source)
		{
			return new SwipeTransitionAnimator (TargetEdge);
		}

		[Export ("animationControllerForDismissedController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController (UIViewController dismissed)
		{
			return new SwipeTransitionAnimator (TargetEdge);
		}

		[Export ("interactionControllerForPresentation:")]
		public IUIViewControllerInteractiveTransitioning GetInteractionControllerForPresentation (IUIViewControllerAnimatedTransitioning animator)
		{
			if (GestureRecognizer != null)
				return new SwipeTransitionInteractionController (GestureRecognizer, TargetEdge);

			return null;
		}

		[Export ("interactionControllerForDismissal:")]
		public IUIViewControllerInteractiveTransitioning GetInteractionControllerForDismissal (IUIViewControllerAnimatedTransitioning animator)
		{
			if (GestureRecognizer != null)
				return new SwipeTransitionInteractionController (GestureRecognizer, TargetEdge);

			return null;
		}
	}
}
