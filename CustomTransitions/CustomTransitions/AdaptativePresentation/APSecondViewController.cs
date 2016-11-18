using System;

using UIKit;

namespace CustomTransitions
{
	public partial class APSecondViewController : UIViewController, IUIAdaptivePresentationControllerDelegate
	{
		public APSecondViewController(IntPtr handle)
			: base (handle)
		{
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UIBarButtonItem dismissButton = new UIBarButtonItem("Dismiss", UIBarButtonItemStyle.Plain, DismissButtonAction);
			NavigationItem.LeftBarButtonItem = dismissButton;
		}


		//public new IUIViewControllerTransitioningDelegate TransitioningDelegate
		//{
		//	set {
		//		base.TransitioningDelegate = value;

		//		if (PresentationController != null)
		//		{
		//			PresentationController.Delegate = this;
		//		}
		//	}
		//}


		void DismissButtonAction(object sender, EventArgs e)
		{
			PerformSegue("UnwindToFirstViewController", (UIBarButtonItem) sender);
		}


		public UIModalPresentationStyle GetAdaptivePresentationStyle(UIPresentationController forPresentationController)
		{
			return UIModalPresentationStyle.FullScreen;
		}

	
		public UIViewController GetViewControllerForAdaptivePresentation(UIPresentationController controller, UIModalPresentationStyle style)
		{
			return new UINavigationController(controller.PresentedViewController);
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

