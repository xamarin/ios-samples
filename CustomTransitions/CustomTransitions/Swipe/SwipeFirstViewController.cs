using System;

using UIKit;

namespace CustomTransitions
{
	public partial class SwipeFirstViewController : UIViewController
	{
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

		//
		void InteractiveTransitionRecognizerAction(UIScreenEdgePanGestureRecognizer sender)
		{
			if (sender.State == UIGestureRecognizerState.Began) { 
				//[self performSegueWithIdentifier:@"CustomTransition" sender:sender];
			}
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			if (segue.Identifier == "") {
				UIViewController destinationVC = segue.DestinationViewController;

				if (sender is UIGestureRecognizer)
				{
					//transitionDelegate.gestureRecognizer = sender;
				}
				else {
					//transitionDelegate.gestureRecognizer = nil;
				}

				if (sender.GetType() == typeof(UIGestureRecognizer))
				{
					//transitionDelegate.gestureRecognizer = sender;
				}
				else {
					//transitionDelegate.gestureRecognizer = nil;
				}

				//transitionDelegate.targetEdge = UIRectEdgeRight;
				//destinationViewController.transitioningDelegate = transitionDelegate;

				destinationVC.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			}
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

