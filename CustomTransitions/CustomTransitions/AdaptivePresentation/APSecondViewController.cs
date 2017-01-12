using System;

using UIKit;

namespace CustomTransitions {
	public partial class APSecondViewController : UIViewController, IUIAdaptivePresentationControllerDelegate {

		public AdaptivePresentationController PresentationControllerInstance { get; set; }

		public new IUIViewControllerTransitioningDelegate TransitioningDelegate {
			set {
				base.TransitioningDelegate = value;

				if (PresentationController != null)
					PresentationController.Delegate = this;
			}
		}

		public APSecondViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var dismissButton = new UIBarButtonItem ("Dismiss", UIBarButtonItemStyle.Plain, DismissButtonAction);
			NavigationItem.LeftBarButtonItem = dismissButton;

			if (PresentationController != null)
				PresentationController.Delegate = this;
		}

		void DismissButtonAction (object sender, EventArgs e)
		{
			PerformSegue ("UnwindToFirstViewController", (UIBarButtonItem) sender);
		}

		public UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController forPresentationController)
		{
			return UIModalPresentationStyle.FullScreen;
		}

		public UIViewController GetViewControllerForAdaptivePresentation (UIPresentationController controller, UIModalPresentationStyle style)
		{
			return new UINavigationController (controller.PresentedViewController);
		}
	}
}

