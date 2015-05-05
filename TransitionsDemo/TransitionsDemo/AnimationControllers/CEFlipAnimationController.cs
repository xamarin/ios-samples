using System;
using UIKit;
using CoreAnimation;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;

namespace TransitionsDemo.AnimationControllers
{
	public class CEFlipAnimationController : CEReversibleAnimationController
	{
		public CEFlipAnimationController ()
		{
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext,
		                                        UIViewController fromViewController, UIViewController toViewController,
		                                        UIView fromView, UIView toView)
		{
			UIView containerView = transitionContext.ContainerView;
			containerView.AddSubview (toView);
			containerView.SendSubviewToBack (toView);

			var transform = CATransform3D.Identity;
			transform.m34 = -0.002f;
			containerView.Layer.SublayerTransform = transform;

			var initialFrame = transitionContext.GetInitialFrameForViewController (fromViewController);
			fromView.Frame = initialFrame;
			toView.Frame = initialFrame;

			List<UIView> toViewSnapshots = CreateSnapshots (toView, true);
			UIView flippedSectionOfToView = toViewSnapshots [Reverse ? 0 : 1];

			List<UIView> fromViewSnapshots = CreateSnapshots (fromView, false);
			UIView flippedSectionOfFromView = fromViewSnapshots [Reverse ? 1 : 0];

			flippedSectionOfFromView = AddShadowToView (flippedSectionOfFromView, !Reverse);
			UIView flippedSectionOfFromViewShadow = flippedSectionOfFromView.Subviews [1];
			flippedSectionOfFromViewShadow.Alpha = 0f;

			flippedSectionOfToView = AddShadowToView (flippedSectionOfToView, Reverse);
			UIView flippedSectionOfToViewShadow = flippedSectionOfToView.Subviews [1];
			flippedSectionOfToViewShadow.Alpha = 1f;

			// change the anchor point so that the view rotate around the correct edge
			UpdateAnchorPointAndOffset (new CGPoint (Reverse ? 0f : 1f, 0.5f), flippedSectionOfFromView);
			UpdateAnchorPointAndOffset (new CGPoint (Reverse ? 1f : 0f, 0.5f), flippedSectionOfToView);

			flippedSectionOfToView.Layer.Transform = Rotate (Reverse ? (float)Math.PI / 2 : -(float)Math.PI / 2);
			double duration = TransitionDuration (transitionContext);

			Action animations = () => {
				UIView.AddKeyframeWithRelativeStartTime (0.0, 0.5, () => {
					flippedSectionOfFromView.Layer.Transform = Rotate (Reverse ? -(float)Math.PI / 2 : (float)Math.PI / 2);
					flippedSectionOfFromViewShadow.Alpha = 1f;
				});

				UIView.AddKeyframeWithRelativeStartTime (0.5, 0.5, () => {
					flippedSectionOfToView.Layer.Transform = Rotate (Reverse ? 0.001f : -0.001f);
					flippedSectionOfToViewShadow.Alpha = 0f;
				});
			};

			UIView.AnimateKeyframes (duration, 0.0, UIViewKeyframeAnimationOptions.CalculationModeLinear, animations, (finished) => {
				if (transitionContext.TransitionWasCancelled) {
					RemoveOtherViews (fromView);
				} else {
					RemoveOtherViews (toView);
				}

				transitionContext.CompleteTransition (!transitionContext.TransitionWasCancelled);
			});
		}

		private void RemoveOtherViews (UIView viewToKeep)
		{
			UIView containerView = viewToKeep.Superview;
			foreach (UIView view in containerView.Subviews) {
				if (view != viewToKeep)
					view.RemoveFromSuperview ();
			}
		}

		private UIView AddShadowToView (UIView view, bool reverse)
		{
			UIView containerView = view.Superview;

			var viewWithShadow = new UIView (view.Frame);
			containerView.InsertSubviewAbove (viewWithShadow, view);
			view.RemoveFromSuperview ();

			var shadowView = new UIView (viewWithShadow.Bounds);

			var colors = new CGColor[] {
				UIColor.FromWhiteAlpha(0f, 0f).CGColor,
				UIColor.FromWhiteAlpha(0f, 0.5f).CGColor
			};

			var gradient = new CAGradientLayer () {
				Frame = shadowView.Bounds,
				Colors =  colors
			};

			gradient.StartPoint = new CGPoint (reverse ? 0f : 1f, 0f);
			gradient.EndPoint = new CGPoint (reverse ? 1f : 0f, 0f);
			shadowView.Layer.InsertSublayer (gradient, 1);

			view.Frame = view.Bounds;
			viewWithShadow.AddSubview (view);

			viewWithShadow.AddSubview (shadowView);
			return viewWithShadow;
		}

		private List<UIView> CreateSnapshots (UIView view, bool afterScreenUpdates)
		{
			UIView containerView = view.Superview;
			var snapshotRegion = new CGRect (0, 0, view.Frame.Size.Width / 2, view.Frame.Size.Height);
			UIView leftHandView = view.ResizableSnapshotView (snapshotRegion, afterScreenUpdates, UIEdgeInsets.Zero);
			leftHandView.Frame = snapshotRegion;
			containerView.AddSubview (leftHandView);

			snapshotRegion = new CGRect (view.Frame.Size.Width / 2, 0, view.Frame.Size.Width / 2, view.Frame.Size.Height);
			UIView rightHandView = view.ResizableSnapshotView (snapshotRegion, afterScreenUpdates, UIEdgeInsets.Zero);
			rightHandView.Frame = snapshotRegion;
			containerView.AddSubview (rightHandView);

			containerView.SendSubviewToBack (view);

			return new List<UIView> () { leftHandView, rightHandView };
		}

		private void UpdateAnchorPointAndOffset (CGPoint anchorPoint, UIView view)
		{
			view.Layer.AnchorPoint = anchorPoint;
			float newX = (float)(view.Frame.X + (anchorPoint.X - 0.5f) * view.Frame.Width);
			view.Frame = new CGRect (newX, view.Frame.Y, view.Frame.Width, view.Frame.Height);
		}

		private CATransform3D Rotate (float angle)
		{
			return CATransform3D.MakeRotation (angle, 0f, 1f, 0f);
		}
	}
}

