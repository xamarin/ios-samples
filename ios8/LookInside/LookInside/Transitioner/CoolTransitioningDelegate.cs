using System;
using System.Globalization;

using UIKit;
using Foundation;

namespace LookInside
{
	public class CoolTransitioningDelegate : UIViewControllerTransitioningDelegate
	{
		UIPresentationController presentationController;
		CoolAnimatedTransitioning present, dismiss;

		public override UIPresentationController GetPresentationControllerForPresentedViewController (
			UIViewController presentedViewController,
			UIViewController presentingViewController,
			UIViewController sourceViewController)
		{
			presentationController = new CoolPresentationController (presentedViewController, presentingViewController);
			return presentationController;
		}

		public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source)
		{
			present = present ?? new CoolAnimatedTransitioning {
				IsPresentation = true
			};
			return present;
		}

		public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController (UIViewController dismissed)
		{
			dismiss = dismiss ?? new CoolAnimatedTransitioning {
				IsPresentation = false
			};
			return dismiss;
		}
	}
}

