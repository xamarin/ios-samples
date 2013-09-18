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

		void InteractionBegan (PointF point)
		{
			UIViewController viewController = ((APLCollectionViewController)navigationController.TopViewController).NextViewControllerAtPoint (point);
			if (viewController != null) {
				navigationController.PushViewController (viewController, true);
			} else {
				navigationController.PopViewControllerAnimated (true);
			}
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
			if (context == null)
				return;

			if ((progress != transitionLayout.TransitionProgress) || (offset != transitionLayout.Offset)) {
				transitionLayout.SetOffset (offset);
				transitionLayout.SetTransitionProgress (progress);
				transitionLayout.InvalidateLayout ();
				context.UpdateInteractiveTransition (progress);
			}
		}

		void EndInteraction (bool success)
		{
			if (context == null) {
				HasActiveInteraction = false;
				return;
			}

			if (success && (transitionLayout.TransitionProgress > 0.5f)) {
				collectionView.FinishInteractiveTransition ();
				context.FinishInteractiveTransition ();
			} else {
				collectionView.CancelInteractiveTransition ();
				context.CancelInteractiveTransition ();
			}
		}

		public void HandlePinch (UIPinchGestureRecognizer sender)
		{
			if (sender.NumberOfTouches < 2)
				return;

			PointF point1 = sender.LocationOfTouch (0, sender.View);
			PointF point2 = sender.LocationOfTouch (1, sender.View);
			float distance = (float) Math.Sqrt ((point1.X - point2.X) * (point1.X - point2.X) +
			                                    (point1.Y - point2.Y) * (point1.Y - point2.Y));
			PointF point = sender.LocationInView (sender.View);

			if (sender.State == UIGestureRecognizerState.Began) {
				if (HasActiveInteraction)
					return;

				initialPinchDistance = distance;
				initialPinchPoint = point;
				HasActiveInteraction = true;
				InteractionBegan (point);
				return;
			}

			if (!HasActiveInteraction)
				return;

			switch (sender.State) {
			case UIGestureRecognizerState.Changed:
				float offsetX = point.X - initialPinchPoint.X;
				float offsetY = point.Y - initialPinchPoint.Y;
				float distanceDelta = distance - initialPinchDistance;

				if (NavigationOperation == UINavigationControllerOperation.Pop)
					distanceDelta = -distanceDelta;

				SizeF size = collectionView.Bounds.Size;
				float dimension = (float)Math.Sqrt (size.Width * size.Width + size.Height * size.Height);
				float progress = (float) Math.Max (Math.Min (distanceDelta / dimension, 1.0), 0.0);
				Update (progress, new UIOffset (offsetX, offsetY));
				break;
			case UIGestureRecognizerState.Ended:
				EndInteraction (true);
				break;
			case UIGestureRecognizerState.Cancelled:
				EndInteraction (false);
				break;
			}
		}

		public void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
		}

		public double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return 1.0f;
		}
	}
}