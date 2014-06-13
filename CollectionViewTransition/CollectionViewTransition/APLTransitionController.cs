using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CollectionViewTransition {

	public class APLTransitionController : UIViewControllerInteractiveTransitioning, IUIViewControllerAnimatedTransitioning {

		APLTransitionLayout transitionLayout;
		UICollectionView collectionView;
		UINavigationController navigationController;
		IUIViewControllerContextTransitioning context;
		float initialPinchDistance;
		PointF initialPinchPoint;

		public APLTransitionController (UICollectionView view, UINavigationController controller)
		{
			collectionView = view;
			collectionView.AddGestureRecognizer (new UIPinchGestureRecognizer (HandlePinch));
			navigationController = controller;
		}

		public bool HasActiveInteraction { get; private set; }

		public UINavigationControllerOperation NavigationOperation { get; set; }

		// required method for view controller transitions, called when the system needs to set up
		// the interactive portions of a view controller transition and start the animations
		void InteractionBegan (PointF point)
		{
			UIViewController viewController = ((APLCollectionViewController)navigationController.TopViewController).NextViewControllerAtPoint (point);
			if (viewController != null) {
				navigationController.PushViewController (viewController, true);
			} else {
				navigationController.PopViewControllerAnimated (true);
			}
		}

		public void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{

		}

		public double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return 1.0f;
		}

		public override void StartInteractiveTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			context = transitionContext;
			var fromVC = (UICollectionViewController) context.GetViewControllerForKey (UITransitionContext.FromViewControllerKey);
			var toVC = (UICollectionViewController) context.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);
			transitionLayout = (APLTransitionLayout) fromVC.CollectionView.StartInteractiveTransition (toVC.Layout, 
				delegate (bool finished, bool completed)  {
				context.CompleteTransition (completed);
				collectionView.WeakDelegate = completed ? toVC : fromVC;
				transitionLayout = null;
				context = null;
				HasActiveInteraction = false;
			});
		}

		void Update (float progress, UIOffset offset)
		{
			if (context != null && // we must have a valid context for updates
				((progress != transitionLayout.TransitionProgress) || (offset != transitionLayout.Offset))) {
				transitionLayout.SetOffset (offset);
				transitionLayout.SetTransitionProgress (progress);
				transitionLayout.InvalidateLayout ();
				context.UpdateInteractiveTransition (progress);
			}
		}

		// called by our pinch gesture recognizer when the gesture has finished or cancelled, which
		// in turn is responsible for finishing or cancelling the transition.
		void EndInteraction (bool success)
		{
			if (context == null) {
				HasActiveInteraction = false;
			}
			// allow for the transition to finish when it's progress has started as a threshold of 10%,
			// if you want to require the pinch gesture with a wider threshold, change it it a value closer to 1.0
			//
			else if ((transitionLayout.TransitionProgress > 0.1f) && success) {
				collectionView.FinishInteractiveTransition ();
				context.FinishInteractiveTransition ();
			} else {
				collectionView.CancelInteractiveTransition ();
				context.CancelInteractiveTransition ();
			}
		}

		// action method for our pinch gesture recognizer
		public void HandlePinch (UIPinchGestureRecognizer sender)
		{
			// here we want to end the transition interaction if the user stops or finishes the pinch gesture
			if (sender.State == UIGestureRecognizerState.Ended) {
				EndInteraction (true);
			} else if (sender.State == UIGestureRecognizerState.Cancelled) {
				EndInteraction (false);
			} else if (sender.NumberOfTouches == 2) {
				// here we expect two finger touch
				PointF point = sender.LocationInView (sender.View); // get the main touch point
				PointF point1 = sender.LocationOfTouch (0, sender.View); // return the locations of each gesture’s touches in the local coordinate system of a given view
				PointF point2 = sender.LocationOfTouch (1, sender.View);
				float distance = (float)Math.Sqrt ((point1.X - point2.X) * (point1.X - point2.X) +
				                (point1.Y - point2.Y) * (point1.Y - point2.Y));

				if (sender.State == UIGestureRecognizerState.Began) {
					// start the pinch in our out
					if (!HasActiveInteraction) {
						initialPinchDistance = distance;
						initialPinchPoint = point;
						HasActiveInteraction = true; // the transition is in active motion
						InteractionBegan (point);
					}
				}

				if (HasActiveInteraction) {
					if (sender.State == UIGestureRecognizerState.Changed) { 
						// update the progress of the transtition as the user continues to pinch
						float offsetX = point.X - initialPinchPoint.X;
						float offsetY = point.Y - initialPinchPoint.Y;

						float distanceDelta = distance - initialPinchDistance;

						if (NavigationOperation == UINavigationControllerOperation.Pop)
							distanceDelta = -distanceDelta;

						SizeF size = collectionView.Bounds.Size;
						float dimension = (float)Math.Sqrt (size.Width * size.Width + size.Height * size.Height);
						float progress = (float)Math.Max (Math.Min (distanceDelta / dimension, 1.0), 0.0);

						// tell our UICollectionViewTransitionLayout subclass (transitionLayout)
						// the progress state of the pinch gesture
						Update (progress, new UIOffset (offsetX, offsetY));
					}
				}
			}
		}
	}
}