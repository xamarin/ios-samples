using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace CustomTransitions
{
	public class SwipeTransitionInteractionController: UIPercentDrivenInteractiveTransition
	{
		public IUIViewControllerContextTransitioning transitionContext;
		public UIScreenEdgePanGestureRecognizer gestureRecognizer;
		UIRectEdge edge;

		public SwipeTransitionInteractionController(UIScreenEdgePanGestureRecognizer gesture, UIRectEdge edge)
		{
			gestureRecognizer = gesture;
			this.edge = edge;

			gestureRecognizer.AddTarget(() => GestureRecognizeDidUpdate(gestureRecognizer));
		}

        public override void StartInteractiveTransition(IUIViewControllerContextTransitioning transitionContext) {
			this.transitionContext = transitionContext;
			base.StartInteractiveTransition(transitionContext);
		}

		float PercentForGesture(UIScreenEdgePanGestureRecognizer gesture) {
			if (this.transitionContext == null) {
				return 0;
			}

			UIView transitionContainerView = transitionContext.ContainerView;
			CGPoint locationInSourceView = gesture.LocationInView(transitionContainerView);

			float width = 0;
			float height = 0;
			if (transitionContainerView != null)
			{
				width = (float)transitionContainerView.Bounds.Width;
				height = (float)transitionContainerView.Bounds.Height;
			}

			if (this.edge == UIRectEdge.Right)
			{
				return ((float)(width - locationInSourceView.X) / width);
			}
			else if (this.edge == UIRectEdge.Left)
			{
				return (float)(locationInSourceView.X / width);
			}
			else if (this.edge == UIRectEdge.Bottom)
			{
				return (float)((height - locationInSourceView.Y) / height);
			}
			else if (this.edge == UIRectEdge.Top)
			{
				return (float)(locationInSourceView.Y / height);
			}
			else { return 0; }
		}


		void GestureRecognizeDidUpdate(UIScreenEdgePanGestureRecognizer sender)
		{

			switch (sender.State){
				case UIGestureRecognizerState.Began:
					break;
				case UIGestureRecognizerState.Changed:
					{
						this.UpdateInteractiveTransition(this.PercentForGesture(sender));
						break;
					}
				case UIGestureRecognizerState.Ended:
					{
						if (this.PercentForGesture(sender) >= 0.5)
						{
							this.FinishInteractiveTransition();
						}
						else {
							this.CancelInteractiveTransition();
						}
						break;
					}
				default:
					this.CancelInteractiveTransition();
					break;
			}
		}



	}
}
