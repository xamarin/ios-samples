using System;
using Foundation;
using UIKit;

namespace CustomTransitions
{
	public partial class CDFirstViewController  : UIViewController, IUIViewControllerTransitioningDelegate
	{
		public CDFirstViewController(IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		partial void presentWithCustomTransitionAction(UIButton sender)
		{
			UIViewController secondViewController = this.Storyboard.InstantiateViewController("SecondViewController");
			secondViewController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			secondViewController.TransitioningDelegate = this;
			this.PresentViewController(secondViewController, true, null);
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
			return (new CrossDissolveTransitionAnimator());
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
			return (new CrossDissolveTransitionAnimator());
		}
	}
}

