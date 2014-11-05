using System;
using CoreGraphics;
using CoreAnimation;
using Foundation;
using UIKit;

namespace TransitionsDemo.AnimationControllers
{
	public class CECardsAnimationController : CEReversibleAnimationController
	{
		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext, 
		                                        UIViewController fromViewController, UIViewController toViewController, 
		                                        UIView fromView, UIView toView)
		{
			if (Reverse) {
				ExecuteReverseAnimation (transitionContext, fromViewController, toViewController, fromView, toView);
			} else {
				ExecuteForwardAnimation (transitionContext, fromViewController, toViewController, fromView, toView);
			}
		}

		private void ExecuteReverseAnimation (IUIViewControllerContextTransitioning transitionContext, 
		                                      UIViewController fromViewController, UIViewController toViewController, 
		                                      UIView fromView, UIView toView)
		{
			UIView containerView = transitionContext.ContainerView;

			// positions the to- view behind the from- view
			toView.Frame = containerView.Frame;
			var scale = CATransform3D.Identity;
			toView.Layer.Transform = scale.Scale (0.6f, 0.6f, 1f);
			toView.Alpha = 0.6f;

			containerView.InsertSubviewAbove (toView, fromView);

			CGRect frameOffScreen = containerView.Frame;
			frameOffScreen.Y = containerView.Frame.Height;

			CATransform3D transform = GetFirstTransform ();

			Action animations = () => {

				// push the from- view off the bottom of the screen
				UIView.AddKeyframeWithRelativeStartTime (0.0, 0.5, () => {
					fromView.Frame = frameOffScreen;
				});

				// animate the to- view into place
				UIView.AddKeyframeWithRelativeStartTime (0.35, 0.35, () => {
					toView.Layer.Transform = transform;
					toView.Alpha = 1f;
				});

				UIView.AddKeyframeWithRelativeStartTime (0.75, 0.25, () => {
					toView.Layer.Transform = CATransform3D.Identity;
				});
			};

			UIView.AnimateKeyframes (Duration, 0.0, UIViewKeyframeAnimationOptions.CalculationModeCubic, animations, (finished) => {
				if (transitionContext.TransitionWasCancelled) {
					toView.Layer.Transform = CATransform3D.Identity;
					toView.Alpha = 1f;
				}

				transitionContext.CompleteTransition (!transitionContext.TransitionWasCancelled);
			});
		}

		private void ExecuteForwardAnimation (IUIViewControllerContextTransitioning transitionContext, 
		                                      UIViewController fromViewController, UIViewController toViewController, 
		                                      UIView fromView, UIView toView)
		{
			UIView containerView = transitionContext.ContainerView;

			// positions the to- view off the bottom of the sceen
			CGRect offScreenFrame = containerView.Frame;
			offScreenFrame.Y = containerView.Frame.Height;
			toView.Frame = offScreenFrame;

			containerView.InsertSubviewAbove (toView, fromView);

			CATransform3D firstTransform = GetFirstTransform ();
			CATransform3D secondTrsnaform = GetSecondTransform (fromView);

			Action animations = () => {
				UIView.AddKeyframeWithRelativeStartTime (0.0, 0.4, () => {
					fromView.Layer.Transform = firstTransform;
					fromView.Alpha = 0.6f;
				});

				UIView.AddKeyframeWithRelativeStartTime (0.2, 0.4, () => {
					fromView.Layer.Transform = secondTrsnaform;
				});

				UIView.AddKeyframeWithRelativeStartTime (0.6, 0.2, () => {
					toView.Frame = new CGRect (toView.Frame.X, containerView.Frame.Y - 30f, 
					                               toView.Frame.Width, toView.Frame.Height);
				});

				UIView.AddKeyframeWithRelativeStartTime (0.8, 0.2, () => {
					toView.Frame = containerView.Frame;
				});
			};

			UIView.AnimateKeyframes (Duration, 0.0, UIViewKeyframeAnimationOptions.CalculationModeCubic, animations, (finished) => {

				transitionContext.CompleteTransition (!transitionContext.TransitionWasCancelled);
			});
		}

		private CATransform3D GetFirstTransform ()
		{
			var firstTransform = CATransform3D.Identity;
			firstTransform.m34 = 1f / -900f;
			firstTransform = firstTransform.Scale (0.95f, 0.95f, 1f);
			firstTransform = firstTransform.Rotate (15.0f * (float)Math.PI / 180f, 1f, 0f, 0f);
			return firstTransform;
		}

		private CATransform3D GetSecondTransform (UIView view)
		{
			var secondTrnsform = CATransform3D.Identity;
			secondTrnsform.m34 = GetFirstTransform ().m34;
			secondTrnsform = secondTrnsform.Translate (0f, view.Frame.Height * -0.08f, 0f);
			secondTrnsform = secondTrnsform.Scale (0.8f, 0.8f, 1f);
			return secondTrnsform;
		}
	}
}

