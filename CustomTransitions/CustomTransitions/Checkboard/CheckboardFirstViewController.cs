using System;
using Foundation;
using UIKit;

namespace CustomTransitions
{
	public partial class CheckboardFirstViewController : UIViewController, IUINavigationControllerDelegate
	{
		partial void unwindToMenuViewController(UIBarButtonItem sender)
		{
			DismissViewController(true, null);
		}

		public CheckboardFirstViewController(IntPtr handle)
			: base (handle)
		{
			NavigationController.Delegate = this;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		[Export("navigationController:animationControllerForOperation:fromViewController:toViewController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation(UINavigationController navigationController, UINavigationControllerOperation animationControllerForOperation, UIViewController fromViewController, UIViewController toViewController) {
			return new CheckboardTransitionAnimator();
		}
	}
}

