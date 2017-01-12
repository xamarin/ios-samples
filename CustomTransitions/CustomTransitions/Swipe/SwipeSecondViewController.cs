using System;

using UIKit;

namespace CustomTransitions {
	public partial class SwipeSecondViewController : UIViewController {

		public SwipeSecondViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var interactiveTransitionRecognizer = new UIScreenEdgePanGestureRecognizer ();
			interactiveTransitionRecognizer.AddTarget (() => InteractiveTransitionRecognizerAction (interactiveTransitionRecognizer));
			interactiveTransitionRecognizer.Edges = UIRectEdge.Left;
			View.AddGestureRecognizer (interactiveTransitionRecognizer);
		}

		void InteractiveTransitionRecognizerAction (UIScreenEdgePanGestureRecognizer sender)
		{
			if (sender.State == UIGestureRecognizerState.Began)
				PerformSegue ("BackToFirstViewController", sender);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			if (segue.Identifier == "BackToFirstViewController") {
				if (TransitioningDelegate is SwipeTransitionDelegate) {
					var transitionDelegate = (SwipeTransitionDelegate)TransitioningDelegate;

					if (sender is UIScreenEdgePanGestureRecognizer)
						transitionDelegate.GestureRecognizer = (UIScreenEdgePanGestureRecognizer)sender;
					else
						transitionDelegate.GestureRecognizer = null;

					transitionDelegate.TargetEdge = UIRectEdge.Left;
				}
			}
		}
	}
}
