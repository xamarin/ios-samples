using System;
using UIKit;
using Foundation;

namespace CustomTransitions
{
	public class SwipeTransitionDelegate : NSObject, IUIViewControllerTransitioningDelegate
	{
		public UIScreenEdgePanGestureRecognizer gestureRecognizer;
		public UIRectEdge targetEdge;

		//| ----------------------------------------------------------------------------
		//  The system calls this method on the presented view controller's
		//  transitioningDelegate to retrieve the animator object used for animating
		//  the presentation of the incoming view controller.  Your implementation is
		//  expected to return an object that conforms to the
		//  UIViewControllerAnimatedTransitioning protocol, or nil if the default
		//  presentation animation should be used.

		[Export("animationControllerForPresentedController:presentingController:sourceController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
		{			
			return new SwipeTransitionAnimator(targetEdge);;
		}

		//| ----------------------------------------------------------------------------
		//  The system calls this method on the presented view controller's
		//  transitioningDelegate to retrieve the animator object used for animating
		//  the dismissal of the presented view controller.  Your implementation is
		//  expected to return an object that conforms to the
		//  UIViewControllerAnimatedTransitioning protocol, or nil if the default
		//  dismissal animation should be used.
		//
		[Export("animationControllerForDismissedController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
		{
			return new SwipeTransitionAnimator(targetEdge); ;
		}

		//| ----------------------------------------------------------------------------
		//  If a <UIViewControllerAnimatedTransitioning> was returned from
		//  -animationControllerForPresentedController:presentingController:sourceController:,
		//  the system calls this method to retrieve the interaction controller for the
		//  presentation transition.  Your implementation is expected to return an
		//  object that conforms to the UIViewControllerInteractiveTransitioning
		//  protocol, or nil if the transition should not be interactive.
		//
		[Export("interactionControllerForPresentation:")]
		public IUIViewControllerInteractiveTransitioning GetInteractionControllerForPresentation(IUIViewControllerAnimatedTransitioning animator)
		{
			if (gestureRecognizer != null)
			{
				return new SwipeTransitionInteractionController(gestureRecognizer, targetEdge);
			}
			else {
				return null;
			}
		}

		//| ----------------------------------------------------------------------------
		//  If a <UIViewControllerAnimatedTransitioning> was returned from
		//  -animationControllerForDismissedController:,
		//  the system calls this method to retrieve the interaction controller for the
		//  dismissal transition.  Your implementation is expected to return an
		//  object that conforms to the UIViewControllerInteractiveTransitioning
		//  protocol, or nil if the transition should not be interactive.
		//
		[Export("interactionControllerForDismissal:")]
		public IUIViewControllerInteractiveTransitioning GetInteractionControllerForDismissal(IUIViewControllerAnimatedTransitioning animator)
		{
			if (gestureRecognizer != null)
			{
				return new SwipeTransitionInteractionController(gestureRecognizer, targetEdge);
			}
			else {
				return null;
			}
		}
	}
}