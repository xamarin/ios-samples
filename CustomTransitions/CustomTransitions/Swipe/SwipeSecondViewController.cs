using System;

using UIKit;

namespace CustomTransitions
{
	public partial class SwipeSecondViewController : UIViewController
	{

		public SwipeSecondViewController(IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UIScreenEdgePanGestureRecognizer interactiveTransitionRecognizer = new UIScreenEdgePanGestureRecognizer();
			interactiveTransitionRecognizer.AddTarget(() => InteractiveTransitionRecognizerAction(interactiveTransitionRecognizer));
			interactiveTransitionRecognizer.Edges = UIRectEdge.Left;
			View.AddGestureRecognizer(interactiveTransitionRecognizer);
		}


		void InteractiveTransitionRecognizerAction(UIScreenEdgePanGestureRecognizer sender)
		{
			if (sender.State == UIGestureRecognizerState.Began)
			{
				PerformSegue("BackToFirstViewController", sender);
			}
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			if (segue.Identifier == "BackToFirstViewController")
			{
				// Check if we were presented with our custom transition delegate.
				// If we were, update the configuration of the
				// AAPLSwipeTransitionDelegate with the gesture recognizer and
				// targetEdge for this view controller.
				if (TransitioningDelegate is SwipeTransitionDelegate) {
					SwipeTransitionDelegate transitionDelegate = (SwipeTransitionDelegate)TransitioningDelegate;

					// If this will be an interactive presentation, pass the gesture
					// recognizer along to our AAPLSwipeTransitionDelegate instance
					// so it can return the necessary
					// <UIViewControllerInteractiveTransitioning> for the presentation.
					if (sender is UIScreenEdgePanGestureRecognizer)
					{
						transitionDelegate.gestureRecognizer = (UIScreenEdgePanGestureRecognizer)sender;
					}
					else {
						transitionDelegate.gestureRecognizer = null;
					}

					transitionDelegate.targetEdge = UIRectEdge.Left;
				}
			}
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}
	}
}

