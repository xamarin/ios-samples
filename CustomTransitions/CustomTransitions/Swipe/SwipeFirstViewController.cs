using System;
using UIKit;

namespace CustomTransitions
{
	public partial class SwipeFirstViewController : UIViewController
	{
		SwipeTransitionDelegate customTransitionDelegate;

		public SwipeFirstViewController(IntPtr handle)
			: base (handle)
		{
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UIScreenEdgePanGestureRecognizer interactiveTransitionRecognizer = new UIScreenEdgePanGestureRecognizer();
			interactiveTransitionRecognizer.AddTarget(() => InteractiveTransitionRecognizerAction(interactiveTransitionRecognizer));
			interactiveTransitionRecognizer.Edges = UIRectEdge.Right;
			View.AddGestureRecognizer(interactiveTransitionRecognizer);
		}


		void InteractiveTransitionRecognizerAction(UIScreenEdgePanGestureRecognizer sender)
		{
			if (sender.State == UIGestureRecognizerState.Began) {
				PerformSegue("CustomTransition", sender);
			}
		}


		public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			if (segue.Identifier == "CustomTransition") {
				UIViewController destinationVC = segue.DestinationViewController;

				SwipeTransitionDelegate transitionDelegate = GetCustomTransitionDelegate();

				if (sender is UIGestureRecognizer)
				{
					transitionDelegate.gestureRecognizer = (UIScreenEdgePanGestureRecognizer) sender;
				}
				else {
					transitionDelegate.gestureRecognizer = null;
				}

				transitionDelegate.targetEdge = UIRectEdge.Right;

				destinationVC.TransitioningDelegate = transitionDelegate;

				destinationVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			}
		}

		public SwipeTransitionDelegate GetCustomTransitionDelegate() {
			if (customTransitionDelegate == null) {
				customTransitionDelegate = new SwipeTransitionDelegate();
			}

			return customTransitionDelegate;
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

