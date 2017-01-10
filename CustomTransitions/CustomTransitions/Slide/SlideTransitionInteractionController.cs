using System;
using UIKit;
using CoreGraphics;

namespace CustomTransitions
{
	public class SlideTransitionInteractionController : UIPercentDrivenInteractiveTransition
	{
		public IUIViewControllerContextTransitioning transitionContext;
		public UIPanGestureRecognizer gestureRecognizer;
		CGPoint initialTranslationInContainerView;


		public SlideTransitionInteractionController(UIPanGestureRecognizer gesture)
		{
			gestureRecognizer = gesture;
			gestureRecognizer.AddTarget(() => GestureRecognizeDidUpdate(gestureRecognizer));
		}


		public override void StartInteractiveTransition(IUIViewControllerContextTransitioning transitionContext)
		{
			this.transitionContext = transitionContext;

			initialTranslationInContainerView = gestureRecognizer.TranslationInView(transitionContext.ContainerView);

			base.StartInteractiveTransition(transitionContext);
		}


		float PercentForGesture(UIPanGestureRecognizer gesture)
		{
			if (transitionContext == null)
			{
				return 0;
			}

			UIView transitionContainerView = transitionContext.ContainerView;

			CGPoint translationInContainerView = gesture.TranslationInView(transitionContainerView);

			if (translationInContainerView.X > 0F && initialTranslationInContainerView.X < 0F
				&& translationInContainerView.X < 0F && initialTranslationInContainerView.X > 0F)
			{
				return -1F;
			}

			float retVal = (float)translationInContainerView.X / (float)transitionContainerView.Bounds.Width;

			return Math.Abs(retVal);
		}


		void GestureRecognizeDidUpdate(UIPanGestureRecognizer sender)
		{	
			switch (sender.State)
			{
				case UIGestureRecognizerState.Began:
					break;
				case UIGestureRecognizerState.Changed:
					{
						if (PercentForGesture(sender) < 0F)
						{
							CancelInteractiveTransition();

							// TODO remove target from gesture
						}
						else {
							UpdateInteractiveTransition(PercentForGesture(sender));
						}
					}
					break;
				case UIGestureRecognizerState.Ended:
					{
						if (PercentForGesture(gestureRecognizer) >= 0.4F)
						{
							FinishInteractiveTransition();
						}
						else {
							CancelInteractiveTransition();
						}
					}
					break;
				default: CancelInteractiveTransition(); break;
			}
		}

	}
}
