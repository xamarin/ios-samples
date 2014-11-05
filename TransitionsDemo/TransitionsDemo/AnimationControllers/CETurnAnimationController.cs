using System;
using CoreGraphics;
using UIKit;
using CoreAnimation;
using Foundation;

namespace TransitionsDemo.AnimationControllers
{
	public enum  CEDirection
	{
		Horizontal = 0,
		Vertical
	}

	public class CETurnAnimationController : CEReversibleAnimationController
	{
		public CEDirection TurnDiresction { get; private set; }

		public CETurnAnimationController ()
		{
			TurnDiresction = CEDirection.Vertical;
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext, 
		                                        UIViewController fromViewController, UIViewController toViewController, 
		                                        UIView fromView, UIView toView)
		{
			UIView containerView = transitionContext.ContainerView;
			containerView.AddSubview (toView);

			// Add a perspective transform
			var transform = CATransform3D.Identity;
			transform.m34 = -0.002f;
			containerView.Layer.SublayerTransform = transform;

			// Give both VCs the same start frame
			CGRect initialFrame = transitionContext.GetInitialFrameForViewController (fromViewController);
			fromView.Frame = initialFrame;
			toView.Frame = initialFrame;

			float factor = Reverse ? 1f : -1f;

			// flip the to VC halfway round - hiding it
			toView.Layer.Transform = Rotate (factor * -(float)Math.PI / 2);
			double duration = TransitionDuration (transitionContext);

			Action animations = () => {
				UIView.AddKeyframeWithRelativeStartTime (0.0, 0.5, () => {
					fromView.Layer.Transform = Rotate (factor * (float)Math.PI / 2);
				});

				UIView.AddKeyframeWithRelativeStartTime (0.5, 0.5, () => {
					toView.Layer.Transform = Rotate (0f);
				});
			};

			UIView.AnimateKeyframes (duration, 0.0, UIViewKeyframeAnimationOptions.CalculationModeLinear, animations, (finished) => {
				transitionContext.CompleteTransition (!transitionContext.TransitionWasCancelled);
			});
		}

		private CATransform3D Rotate (float angle)
		{
			if (TurnDiresction == CEDirection.Horizontal) {
				return  CATransform3D.MakeRotation (angle, 1f, 0f, 0f);
			} else {
				return  CATransform3D.MakeRotation (angle, 0f, 1f, 0f);
			}
		}
	}
}

