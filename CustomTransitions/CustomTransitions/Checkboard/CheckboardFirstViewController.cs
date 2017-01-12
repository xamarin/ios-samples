using System;

using Foundation;
using UIKit;

namespace CustomTransitions {
	public partial class CheckboardFirstViewController : UIViewController, IUINavigationControllerDelegate {
		partial void unwindToMenuViewController(UIBarButtonItem sender)
		{
			DismissViewController (true, null);
		}

		public CheckboardFirstViewController (IntPtr handle) : base (handle)
		{
			NavigationController.Delegate = this;
		}

		[Export ("navigationController:animationControllerForOperation:fromViewController:toViewController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation (UINavigationController navigationController, UINavigationControllerOperation animationControllerForOperation, UIViewController fromViewController, UIViewController toViewController)
		{
			return new CheckboardTransitionAnimator ();
		}
	}
}

