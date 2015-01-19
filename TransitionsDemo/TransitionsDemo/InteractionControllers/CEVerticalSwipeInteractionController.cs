using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace TransitionsDemo.InteractionControllers
{
	public class CEVerticalSwipeInteractionController : CEBaseInteractionController
	{
		public override nfloat CompletionSpeed {
			get {
				return 1 - PercentComplete;
			}
		}

		public override void WireToViewController (UIViewController viewController, CEInteractionOperation operation)
		{
			if (operation == CEInteractionOperation.Tab)
				throw new Exception ("You cannot use a vertical swipe interaction with a tabbar controller - that would be silly!");

			this.operation = operation;
			this.viewController = viewController;
			PrepareGestureRecognizerInView (viewController.View);
		}

		private void PrepareGestureRecognizerInView (UIView view)
		{
			gestureRecognizer = new UIPanGestureRecognizer (this, new Selector ("HandleGesture:"));
			view.AddGestureRecognizer (gestureRecognizer);
		}

		[Export("HandleGesture:")]
		public void HandleGesture (UIPanGestureRecognizer gestureRecognizer)
		{
			CGPoint translation = gestureRecognizer.TranslationInView (gestureRecognizer.View.Superview);

			switch (gestureRecognizer.State) {
			case UIGestureRecognizerState.Began:
				bool topToBottomSwipe = translation.Y > 0f;
				if (operation == CEInteractionOperation.Pop) {
					if (topToBottomSwipe) {
						InteractionInProgress = true;
						viewController.NavigationController.PopViewController (true);
					}
				} else {
					InteractionInProgress = true;
					viewController.DismissViewController (true, null);
				}
				break;
			case UIGestureRecognizerState.Changed:
				if (InteractionInProgress) {
					// compute the current position
					float fraction = (float)translation.Y / 200f;
					fraction = Math.Min (Math.Max (fraction, 0f), 1f);
					shouldCompleteTransition = (fraction > 0.5f);

					UpdateInteractiveTransition (fraction);
				}
				break;
			case UIGestureRecognizerState.Ended:
			case UIGestureRecognizerState.Cancelled:
				if (InteractionInProgress) {
					InteractionInProgress = false;
					if (!shouldCompleteTransition || gestureRecognizer.State == UIGestureRecognizerState.Cancelled) {
						CancelInteractiveTransition ();
					} else {
						FinishInteractiveTransition ();
					}
				}
				break;
			}
		}
	}
}

