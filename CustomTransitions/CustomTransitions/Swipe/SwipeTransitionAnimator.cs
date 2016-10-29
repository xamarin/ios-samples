using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace CustomTransitions
{
	public class SwipeTransitionAnimator : UIViewControllerAnimatedTransitioning
	{
		public UIRectEdge targetEdge;

		public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
		{
			return 0.35;
		}

		public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
		{ 
			UIViewController fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
			UIViewController toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

			UIView containerView = transitionContext.ContainerView;

			// In iOS 8, the viewForKey: method was introduced to get views that the
			// animator manipulates.  This method should be preferred over accessing
			// the view of the fromViewController/toViewController directly.
			// It may return nil whenever the animator should not touch the view
			// (based on the presentation style of the incoming view controller).
			// It may also return a different view for the animator to animate.
			UIView fromView = transitionContext.GetViewFor(UITransitionContext.FromViewKey);
			UIView toView = transitionContext.GetViewFor(UITransitionContext.ToViewKey);

			bool isPresenting = (toViewController.PresentingViewController == fromViewController);

			CGRect fromFrame = transitionContext.GetInitialFrameForViewController(fromViewController);
			CGRect toFrame = transitionContext.GetFinalFrameForViewController(toViewController);

			CGVector offset = new CGVector(0, 0);
			if (this.targetEdge == UIRectEdge.Top)
			{
				offset = new CGVector(0, 1);
			}
			else if (this.targetEdge == UIRectEdge.Bottom)
			{
				offset = new CGVector(0, -1);
			}
			else if (this.targetEdge == UIRectEdge.Left)
			{
				offset = new CGVector(1, 0);
			}
			else if (this.targetEdge == UIRectEdge.Right) 
			{
				offset = new CGVector(-1, 0);
			}
			else { 
				offset = new CGVector(0, 0);
				//TODO: error
			}

			if (isPresenting)
			{
				fromView.Frame = fromFrame;
				toView.Frame = new CGRect(toFrame.X + (toFrame.Size.Width * offset.dx * -1), toFrame.Y + (toFrame.Size.Height * offset.dy * -1), toFrame.Size.Width, toFrame.Size.Height);
			}
			else {
				fromView.Frame = fromFrame;
				toView.Frame = toFrame;
			}


			// We are responsible for adding the incoming view to the containerView
			// for the presentation.
			if (isPresenting)
			{
				containerView.AddSubview(toView);
			}
			else {
				// -addSubview places its argument at the front of the subview stack.
				// For a dismissal animation we want the fromView to slide away,
				// revealing the toView.  Thus we must place toView under the fromView.
				containerView.InsertSubviewBelow(toView, fromView);
			}

			double duration = this.TransitionDuration(transitionContext);

			UIView.Animate(duration, 0, UIViewAnimationOptions.TransitionNone,
				() =>
				{
					if (isPresenting)
					{
						toView.Frame = toFrame;
					}
					else { 
					fromView.Frame = new CGRect(fromFrame.X + (fromFrame.Size.Width * offset.dx), fromFrame.Y + (fromFrame.Size.Height * offset.dy), fromFrame.Size.Width, fromFrame.Size.Height);
				}
				},
				() =>
				{
					bool wasCancel = transitionContext.TransitionWasCancelled;
					if (wasCancel) {
						toView.RemoveFromSuperview();
					}
					transitionContext.CompleteTransition(!wasCancel);
				}
			);
		}	
	}

}
