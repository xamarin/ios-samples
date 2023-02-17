using UIKit;
using System;

namespace CustomTransitions {
	public class SwipeTransitionInteractionController : UIPercentDrivenInteractiveTransition {

		IUIViewControllerContextTransitioning TransitionContext { get; set; }

		UIScreenEdgePanGestureRecognizer GestureRecognizer { get; set; }

		UIRectEdge Edge { get; set; }

		public SwipeTransitionInteractionController (UIScreenEdgePanGestureRecognizer gestureRecognizer, UIRectEdge edge)
		{
			GestureRecognizer = gestureRecognizer;
			Edge = edge;

			GestureRecognizer.AddTarget (() => GestureRecognizeDidUpdate (GestureRecognizer));
		}

		public override void StartInteractiveTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			TransitionContext = transitionContext;
			base.StartInteractiveTransition (transitionContext);
		}

		nfloat PercentForGesture (UIScreenEdgePanGestureRecognizer gesture)
		{
			if (TransitionContext == null)
				return 0f;

			UIView transitionContainerView = TransitionContext.ContainerView;
			var locationInSourceView = gesture.LocationInView (transitionContainerView);

			nfloat width = 0f;
			nfloat height = 0f;

			if (transitionContainerView != null) {
				width = transitionContainerView.Bounds.Width;
				height = transitionContainerView.Bounds.Height;
			}

			if (Edge == UIRectEdge.Right)
				return (width - locationInSourceView.X) / width;

			if (Edge == UIRectEdge.Left)
				return locationInSourceView.X / width;

			if (Edge == UIRectEdge.Bottom)
				return (height - locationInSourceView.Y) / height;

			if (Edge == UIRectEdge.Top)
				return locationInSourceView.Y / height;

			return 0;
		}

		void GestureRecognizeDidUpdate (UIScreenEdgePanGestureRecognizer sender)
		{
			switch (sender.State) {
			case UIGestureRecognizerState.Changed:
				UpdateInteractiveTransition (PercentForGesture (sender));
				break;
			case UIGestureRecognizerState.Ended:
				if (PercentForGesture (sender) >= 0.5)
					FinishInteractiveTransition ();
				else
					CancelInteractiveTransition ();
				break;
			default:
				CancelInteractiveTransition ();
				break;
			}
		}
	}
}
