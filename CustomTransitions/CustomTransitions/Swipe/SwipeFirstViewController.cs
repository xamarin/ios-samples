using System;

using UIKit;

namespace CustomTransitions {
	public partial class SwipeFirstViewController : UIViewController {
		SwipeTransitionDelegate customTransitionDelegate;

		public SwipeFirstViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var interactiveTransitionRecognizer = new UIScreenEdgePanGestureRecognizer ();
			interactiveTransitionRecognizer.AddTarget (() => InteractiveTransitionRecognizerAction (interactiveTransitionRecognizer));
			interactiveTransitionRecognizer.Edges = UIRectEdge.Right;
			View.AddGestureRecognizer (interactiveTransitionRecognizer);
		}

		void InteractiveTransitionRecognizerAction (UIScreenEdgePanGestureRecognizer sender)
		{
			if (sender.State == UIGestureRecognizerState.Began)
				PerformSegue ("CustomTransition", sender);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			if (segue.Identifier == "CustomTransition") {
				UIViewController destinationVC = segue.DestinationViewController;
				var transitionDelegate = GetCustomTransitionDelegate ();

				if (sender is UIGestureRecognizer)
					transitionDelegate.GestureRecognizer = (UIScreenEdgePanGestureRecognizer) sender;
				else
					transitionDelegate.GestureRecognizer = null;

				transitionDelegate.TargetEdge = UIRectEdge.Right;
				destinationVC.TransitioningDelegate = transitionDelegate;
				destinationVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			}
		}

		public SwipeTransitionDelegate GetCustomTransitionDelegate ()
		{
			customTransitionDelegate = customTransitionDelegate ?? new SwipeTransitionDelegate ();
			return customTransitionDelegate;
		}
	}
}
