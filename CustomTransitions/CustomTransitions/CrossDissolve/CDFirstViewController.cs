using System;

using Foundation;
using UIKit;

namespace CustomTransitions {
	public partial class CDFirstViewController : UIViewController, IUIViewControllerTransitioningDelegate {
		public CDFirstViewController (IntPtr handle) : base (handle)
		{
		}

		partial void PresentWithCustomTransitionAction (UIButton sender)
		{
			UIViewController secondViewController = Storyboard.InstantiateViewController ("SecondViewController");
			secondViewController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			secondViewController.TransitioningDelegate = this;
			PresentViewController (secondViewController, true, null);
		}

		[Export ("animationControllerForPresentedController:presentingController:sourceController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source)
		{
			return new CrossDissolveTransitionAnimator ();
		}

		[Export ("animationControllerForDismissedController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController (UIViewController dismissed)
		{
			return new CrossDissolveTransitionAnimator ();
		}
	}
}
