using System;
using UIKit;
using Foundation;

namespace CustomTransitions
{
	public class SwipeTransitionInteractionController: UIPercentDrivenInteractiveTransition
	{
		public UIViewControllerContextTransitioning transitionContext;
		public UIScreenEdgePanGestureRecognizer gestureRecognizer;
		UIRectEdge edge;

		public SwipeTransitionInteractionController(UIScreenEdgePanGestureRecognizer gesture, UIRectEdge edge)
		{
			this.gestureRecognizer = gesture;
			this.edge = edge;



			//UIScreenEdgePanGestureRecognizer interactiveTransitionRecognizer = new UIScreenEdgePanGestureRecognizer();
			//interactiveTransitionRecognizer.AddTarget(() => InteractiveTransitionRecognizerAction(interactiveTransitionRecognizer));
			//interactiveTransitionRecognizer.Edges = UIRectEdge.Right;
			//View.AddGestureRecognizer(interactiveTransitionRecognizer);


		}

		void gestureRecognizeDidUpdate(UIScreenEdgePanGestureRecognizer sender)
		{
			if (sender.State == UIGestureRecognizerState.Began)
			{
				//[self performSegueWithIdentifier:@"CustomTransition" sender:sender];
			}
		}
	}
}
