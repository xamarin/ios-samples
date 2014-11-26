using System;
using System.Globalization;

using UIKit;
using Foundation;

namespace LookInside
{
	public class OverlayTransitioningDelegate : UIViewControllerTransitioningDelegate
	{
		UIPresentationController presentationController;
		OverlayAnimatedTransitioning present, dismiss;

		public override UIPresentationController GetPresentationControllerForPresentedViewController (
			UIViewController presentedViewController,
			UIViewController presentingViewController,
			UIViewController sourceViewController)
		{
			presentationController = presentationController ?? new OverlayPresentationController (presentedViewController, presentingViewController);
			return presentationController;
		}

		public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source)
		{
			present = present ?? new OverlayAnimatedTransitioning {
				IsPresentation = true
			};
			return present;
		}

		public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController (UIViewController dismissed)
		{
			dismiss = dismiss ?? new OverlayAnimatedTransitioning {
				IsPresentation = false
			};
			return dismiss;
		}
	}
}

