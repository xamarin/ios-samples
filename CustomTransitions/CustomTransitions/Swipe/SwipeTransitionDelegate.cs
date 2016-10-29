using System;
using UIKit;
using Foundation;

namespace CustomTransitions
{
	public class SwipeTransitionDelegate : NSObject, IUIViewControllerTransitioningDelegate
	{
		public UIScreenEdgePanGestureRecognizer gestureRecognizer;
		public UIRectEdge targetEdge;

		public SwipeTransitionDelegate(IntPtr handle) : base (handle)
        {
		}

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
			SwipeTransitionAnimator animator = new SwipeTransitionAnimator();
			animator.targetEdge = this.targetEdge;
			return animator;
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
			SwipeTransitionAnimator animator = new SwipeTransitionAnimator();
			animator.targetEdge = this.targetEdge;
			return animator;
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
			if (this.gestureRecognizer != null) { 
			
			}

			return null;
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
			if (this.gestureRecognizer != null)
			{

			}

			return null;
		}
	}
}