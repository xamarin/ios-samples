using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

namespace TransitionsDemo.InteractionControllers
{
	public class CEHorizontalSwipeInteractionController : CEBaseInteractionController
	{
		public override float CompletionSpeed {
			get {
				return 1 - PercentComplete;
			}
		}

		public override void WireToViewController (MonoTouch.UIKit.UIViewController viewController, 
		                                           CEInteractionOperation operation)
		{
			this.operation = operation;
			this.viewController = viewController;
			PrepareGestureRecognizer (viewController.View);
		}

		private void PrepareGestureRecognizer (UIView view)
		{
			gestureRecognizer = new UIPanGestureRecognizer (this, new Selector ("HandleGesture"));
			view.AddGestureRecognizer (gestureRecognizer);
		}

		[Export("HandleGesture")]
		public void HandleGesture (UIPanGestureRecognizer panGestureRecognizer)
		{
			PointF translation = panGestureRecognizer.TranslationInView (panGestureRecognizer.View.Superview);

			switch (gestureRecognizer.State) {
			case UIGestureRecognizerState.Began:
				TrackGestureBegan (translation);
				break;
			case UIGestureRecognizerState.Changed:
				TrackGestureChaged (translation);
				break;
			case UIGestureRecognizerState.Ended:
			case UIGestureRecognizerState.Cancelled:
				TrackGestureEnded (panGestureRecognizer.State);
				break;
			}
		}

		private void TrackGestureBegan (PointF translation)
		{
			bool rightToLeftSwipe = translation.X < 0;

			if (operation == CEInteractionOperation.Pop && rightToLeftSwipe) {
				InteractionInProgress = true;
				viewController.NavigationController.PopViewControllerAnimated (true);
			} else if (operation == CEInteractionOperation.Tab) {
				if (rightToLeftSwipe && 
					viewController.TabBarController.SelectedIndex < viewController.TabBarController.ViewControllers.Length - 1) {
					InteractionInProgress = true;
					viewController.TabBarController.SelectedIndex++;
				} else if (viewController.TabBarController.SelectedIndex > 0) {
					InteractionInProgress = true;
					viewController.TabBarController.SelectedIndex--;
				}
			} else {
				InteractionInProgress = true;
				viewController.DismissViewController (true, null);
			}
		}

		private void TrackGestureChaged (PointF translation)
		{
			if (InteractionInProgress) {
				float fraction = translation.X / 200f;
				fraction = Math.Min (Math.Max (fraction, 0f), 1f);
				shouldCompleteTransition = (fraction > 0.5f);

				UpdateInteractiveTransition (fraction);
			}
		}

		private void TrackGestureEnded (UIGestureRecognizerState state)
		{
			if (InteractionInProgress) {
				InteractionInProgress = false;

				if (!shouldCompleteTransition || state == UIGestureRecognizerState.Cancelled) {
					CancelInteractiveTransition ();
				} else {
					FinishInteractiveTransition ();
				}
			}
		}
	}
}

