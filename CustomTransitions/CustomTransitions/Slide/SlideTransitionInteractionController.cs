using System;

using CoreGraphics;
using UIKit;

namespace CustomTransitions {
	public class SlideTransitionInteractionController : UIPercentDrivenInteractiveTransition {

		readonly UIPanGestureRecognizer gestureRecognizer;

		CGPoint initialTranslationInContainerView;

		IUIViewControllerContextTransitioning TransitionContext { get; set; }

		public SlideTransitionInteractionController (UIPanGestureRecognizer gesture)
		{
			gestureRecognizer = gesture;
			gestureRecognizer.AddTarget (() => GestureRecognizeDidUpdate (gestureRecognizer));
		}

		public override void StartInteractiveTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			TransitionContext = transitionContext;
			initialTranslationInContainerView = gestureRecognizer.TranslationInView (transitionContext.ContainerView);
			base.StartInteractiveTransition (transitionContext);
		}

		nfloat PercentForGesture (UIPanGestureRecognizer gesture)
		{
			if (TransitionContext == null)
				return 0f;

			UIView transitionContainerView = TransitionContext.ContainerView;
			var translationInContainerView = gesture.TranslationInView (transitionContainerView);

			if (translationInContainerView.X > 0f && initialTranslationInContainerView.X < 0f
				&& translationInContainerView.X < 0f && initialTranslationInContainerView.X > 0f)
				return -1f;

			nfloat retVal = translationInContainerView.X / transitionContainerView.Bounds.Width;
			return NMath.Abs (retVal);
		}

		void GestureRecognizeDidUpdate (UIPanGestureRecognizer sender)
		{
			switch (sender.State) {
			case UIGestureRecognizerState.Changed:
				if (PercentForGesture (sender) < 0f)
					CancelInteractiveTransition ();
				else
					UpdateInteractiveTransition (PercentForGesture (sender));
				break;
			case UIGestureRecognizerState.Ended:
				if (PercentForGesture (gestureRecognizer) >= .4f) {
					FinishInteractiveTransition ();
				} else {
					CancelInteractiveTransition ();
				}
				break;
			default:
				CancelInteractiveTransition ();
				break;
			}
		}
	}
}
