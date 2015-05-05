using System;
using UIKit;
using CoreAnimation;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;

namespace TransitionsDemo.AnimationControllers
{
	public class CEFoldAnimationController : CEReversibleAnimationController
	{
		public int Folds { get; private set; }

		public CEFoldAnimationController ()
		{
			Folds = 2;
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext,
		                                        UIViewController fromViewController, UIViewController toViewController,
		                                        UIView fromView, UIView toView)
		{
			// Add the toView to the container
			UIView containerView = transitionContext.ContainerView;

			toView.Center = new CGPoint (1000f, 0f);
			containerView.AddSubview (toView);

			// Add a perspective transform
			var transform = CATransform3D.Identity;
			transform.m34 = -0.005f;
			containerView.Layer.SublayerTransform = transform;

			CGSize size = toView.Frame.Size;

			float foldWidth = (float)(size.Width * 0.5f / Folds);

			// arrays that hold the snapshot views
			var fromViewFolds = new List<UIView> ();
			var toViewFolds = new List<UIView> ();

			for (int i = 0; i < Folds; i++) {
				float offset = i * foldWidth * 2;

				// the left and right side of the fold for the from- view, with identity transform and 0.0 alpha
				// on the shadow, with each view at its initial position
				UIView leftFromViewFold = CreateSnapshot (fromView, false, offset, true);
				leftFromViewFold.Layer.Position = new CGPoint (offset, size.Height / 2f);
				fromViewFolds.Add (leftFromViewFold);
				leftFromViewFold.Subviews [1].Alpha = 0f;

				UIView rightFromViewFold = CreateSnapshot (fromView, false, offset + foldWidth, false);
				rightFromViewFold.Layer.Position = new CGPoint (offset + foldWidth * 2f, size.Height / 2f);
				fromViewFolds.Add (rightFromViewFold);
				rightFromViewFold.Subviews [1].Alpha = 0f;

				// the left and right side of the fold for the to- view, with a 90-degree transform and 1.0 alpha
				// on the shadow, with each view positioned at the very edge of the screen
				UIView leftToViewFold = CreateSnapshot (toView, true, offset, true);
				leftToViewFold.Layer.Position = new CGPoint (Reverse ? size.Width : 0f, size.Height / 2f);
				leftToViewFold.Layer.Transform = CATransform3D.MakeRotation ((float)Math.PI / 2f, 0f, 1f, 0f);
				toViewFolds.Add (leftToViewFold);

				UIView rightToViewFold = CreateSnapshot (toView, true, offset + foldWidth, false);
				rightToViewFold.Layer.Position = new CGPoint (Reverse ? size.Width : 0f, size.Height / 2f);
				rightToViewFold.Layer.Transform = CATransform3D.MakeRotation (-(float)Math.PI / 2f, 0f, 1f, 0f);
				toViewFolds.Add (rightToViewFold);
			}

			//move the from- view off screen
			fromView.Center = new CGPoint (1000f, 0f);

			// create the animation
			double duration = TransitionDuration (transitionContext);

			Action animation = () => {
				for (int i = 0; i < Folds; i++) {

					float offset = i * foldWidth * 2;

					// the left and right side of the fold for the from- view, with 90 degree transform and 1.0 alpha
					// on the shadow, with each view positioned at the edge of thw screen.
					UIView leftFromView = fromViewFolds [i * 2];
					leftFromView.Layer.Position = new CGPoint (Reverse ? 0f : size.Width, size.Height / 2f);
					leftFromView.Layer.Transform = transform.Rotate ((float)Math.PI / 2f, 0f, 1f, 0f);
					leftFromView.Subviews [1].Alpha = 1f;

					UIView rightFromView = fromViewFolds [i * 2 + 1];
					rightFromView.Layer.Position = new CGPoint (Reverse ? 0f : size.Width, size.Height / 2f);
					rightFromView.Layer.Transform = transform.Rotate (-(float)Math.PI / 2f, 0f, 1f, 0f);
					rightFromView.Subviews [1].Alpha = 1f;

					// the left and right side of the fold for the to- view, with identity transform and 0.0 alpha
					// on the shadow, with each view at its final position
					UIView leftToView = toViewFolds [i * 2];
					leftToView.Layer.Position = new CGPoint (offset, size.Height / 2f);
					leftToView.Layer.Transform = CATransform3D.Identity;
					leftToView.Subviews [1].Alpha = 0f;

					UIView rightToView = toViewFolds [i * 2 + 1];
					rightToView.Layer.Position = new CGPoint (offset + foldWidth * 2f, size.Height / 2f);
					rightToView.Layer.Transform = CATransform3D.Identity;
					rightToView.Subviews [1].Alpha = 0f;
				}
			};

			UIView.Animate (duration, animation, () => {
				// remove the snapshot views
				foreach (UIView view in toViewFolds) {
					view.RemoveFromSuperview ();
				}

				foreach (UIView view in fromViewFolds) {
					view.RemoveFromSuperview ();
				}

				// restore the to- and from- to the initial location
				toView.Frame = containerView.Bounds;
				fromView.Frame = containerView.Bounds;
				transitionContext.CompleteTransition (!transitionContext.TransitionWasCancelled);
			});

		}

		private UIView CreateSnapshot (UIView view, bool afterUpdates, float offset, bool left)
		{
			CGSize size = view.Frame.Size;
			UIView containerView = view.Superview;
			float foldWidth = (float)(size.Width * 0.5f / Folds);

			UIView snapshotView;

			if (!afterUpdates) {
				// create a regular snapshot
				var snapshotRegion = new CGRect (offset, 0f, foldWidth, size.Height);
				snapshotView = view.ResizableSnapshotView (snapshotRegion, afterUpdates, UIEdgeInsets.Zero);
			} else {
				// for the to- view for some reason the snapshot takes a while to create. Here we place the snapshot within
				// another view, with the same bckground color, so that it is less noticeable when the snapshot initially renders
				snapshotView = new UIView (new CGRect (0f, 0f, foldWidth, size.Height));
				snapshotView.BackgroundColor = view.BackgroundColor;
				var snapshotRegion = new CGRect (offset, 0f, foldWidth, size.Height);
				UIView snapshotView2 = view.ResizableSnapshotView (snapshotRegion, afterUpdates, UIEdgeInsets.Zero);
				snapshotView.AddSubview (snapshotView2);
			}

			// create a shadow
			UIView snapshotWithShadowView = AddShadowToView (snapshotView, left);

			// add to the container
			containerView.AddSubview (snapshotWithShadowView);

			// set the anchor to the left or right edge of the view
			snapshotWithShadowView.Layer.AnchorPoint = new CGPoint (left ? 0f : 1f, 0.5f);

			return snapshotWithShadowView;
		}

		private UIView AddShadowToView (UIView view, bool left)
		{
			// create a view with the same frame
			var viewWithShadow = new UIView (view.Frame);

			// create a shadow
			var shadowView = new UIView (viewWithShadow.Bounds);

			var colors = new CGColor[] {
				UIColor.FromWhiteAlpha(0f, 0f).CGColor,
				UIColor.FromWhiteAlpha(0f, 1f).CGColor
			};

			var gradient = new CAGradientLayer () {
				Frame = shadowView.Bounds,
				Colors = colors
			};

			gradient.StartPoint = new CGPoint (Reverse ? 0f : 1f, Reverse ? 0.2f : 0f);
			gradient.EndPoint = new CGPoint (Reverse ? 1f : 0f, Reverse ? 0f : 1f);
			shadowView.Layer.InsertSublayer (gradient, 1);

			// add the original view into our new view
			view.Frame = view.Bounds;
			viewWithShadow.AddSubview (view);

			// place the shadow on top
			viewWithShadow.AddSubview (shadowView);

			return viewWithShadow;
		}
	}
}

