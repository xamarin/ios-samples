using System;

using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace CustomTransitions {
	public class CheckboardTransitionAnimator : UIViewControllerAnimatedTransitioning {
		public override double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return 3.0;
		}

		public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			var fromViewController = transitionContext.GetViewControllerForKey (UITransitionContext.FromViewControllerKey);
			var toViewController = transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);
			UIView containerView = transitionContext.ContainerView;

			var fromView = transitionContext.GetViewFor (UITransitionContext.FromViewKey);
			var toView = transitionContext.GetViewFor (UITransitionContext.ToViewKey);

			fromView.Frame = transitionContext.GetInitialFrameForViewController (fromViewController);
			toView.Frame = transitionContext.GetFinalFrameForViewController (toViewController);
			bool isPush = Array.IndexOf (toViewController.NavigationController.ViewControllers, toViewController) > Array.IndexOf (fromViewController.NavigationController.ViewControllers, fromViewController);
			containerView.AddSubview (toView);

			// TODO
			UIImage fromViewSnapshot;
			var toViewSnapshot = new UIImage();

			UIGraphics.BeginImageContextWithOptions (containerView.Bounds.Size, true, containerView.Window.Screen.Scale);
			fromView.DrawViewHierarchy (containerView.Bounds, false);
			fromViewSnapshot = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			CoreFoundation.DispatchQueue.MainQueue.DispatchAsync (() => { 
				UIGraphics.BeginImageContextWithOptions (containerView.Bounds.Size, true, containerView.Window.Screen.Scale);
				toView.DrawViewHierarchy (containerView.Bounds, false);
				toViewSnapshot = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();
			});

			var transitionContainer = new UIView (containerView.Bounds) {
				Opaque = true,
				BackgroundColor = UIColor.Black
			};
			containerView.AddSubview (transitionContainer);

			CATransform3D t = CATransform3D.Identity;
			t.m34 = 1f / -900f;
			transitionContainer.Layer.SublayerTransform = t;

			// The size and number of slices is a function of the width.
			var sliceSize = Math.Round (transitionContainer.Frame.Width / 10f);
			var horizontalSileces = Math.Ceiling (transitionContainer.Frame.Width / sliceSize);
			var verticalSlices = Math.Ceiling (transitionContainer.Frame.Height / sliceSize);

			// transitionSpacing controls the transition duration for each slice.
			// Higher values produce longer animations with multiple slices having
			// their animations 'in flight' simultaneously.
			float transitionSpacing = 160f;
			var transitionDuration = TransitionDuration (transitionContext);

			nfloat dx = isPush ? transitionContainer.Bounds.GetMaxY () - transitionContainer.Bounds.GetMinX ()
			                                        : transitionContainer.Bounds.GetMinX () - transitionContainer.Bounds.GetMaxX ();

			nfloat dy = isPush ? transitionContainer.Bounds.GetMaxY () - transitionContainer.Bounds.GetMinY () :
			                                        transitionContainer.Bounds.GetMinY () - transitionContainer.Bounds.GetMaxY ();

			var transitionVector = new CGVector (dx, dy);

			var transitionVectorLength = (nfloat)Math.Sqrt (transitionVector.dx * transitionVector.dx + transitionVector.dy * transitionVector.dy);
			var transitionUnitVector = new CGVector (transitionVector.dx / transitionVectorLength, transitionVector.dy / new nfloat(transitionVectorLength));

			for (int y = 0; y < verticalSlices; y++) {
				for (int x = 0; x < horizontalSileces; x++) {
					var fromContentLayer = new CALayer {
						Frame = new CGRect (x + sliceSize * -1f, y * sliceSize * -1f, containerView.Bounds.Width, containerView.Bounds.Height),
						RasterizationScale = fromViewSnapshot.CurrentScale,
						Contents = fromViewSnapshot.CGImage
					};

					var toContentLayer = new CALayer ();
					toContentLayer.Frame = new CGRect (x * sliceSize * -1f, y * sliceSize * -1f, containerView.Bounds.Width, containerView.Bounds.Height);

					CoreFoundation.DispatchQueue.MainQueue.DispatchAsync (() => {
						bool wereActionDisabled = CATransaction.DisableActions;
						CATransaction.DisableActions = true;

						toContentLayer.RasterizationScale = toViewSnapshot.CurrentScale;
						toContentLayer.Contents = toViewSnapshot.CGImage;

						CATransaction.DisableActions = wereActionDisabled;
					});

					var toCheckboardSquareView = new UIView {
						Frame = new CGRect (x * sliceSize, y * sliceSize, sliceSize, sliceSize),
						Opaque = false
					};

					toCheckboardSquareView.Layer.MasksToBounds = true;
					toCheckboardSquareView.Layer.DoubleSided = false;
					toCheckboardSquareView.Layer.Transform = CATransform3D.MakeRotation (NMath.PI, 0f, 1f, 0f);
					toCheckboardSquareView.Layer.AddSublayer(toContentLayer);

					var fromCheckboardSquareView = new UIView {
						Frame = new CGRect (x * sliceSize, y * sliceSize, sliceSize, sliceSize),
						Opaque = false
					};

					fromCheckboardSquareView.Layer.MasksToBounds = true;
					fromCheckboardSquareView.Layer.DoubleSided = false;
					fromCheckboardSquareView.Layer.Transform = CATransform3D.Identity;
					fromCheckboardSquareView.Layer.AddSublayer (fromContentLayer);

					transitionContainer.AddSubview (toCheckboardSquareView);
					transitionContainer.AddSubview (fromCheckboardSquareView);
				}
			}

			int sliceAnimationsPending = 0;

			for (int y = 0; y < verticalSlices; y++) {
				for (int x = 0; x < horizontalSileces; x++) {
					double toIndex = y * horizontalSileces * 2f + (x * 2);
					UIView toCheckboardSquareView = transitionContainer.Subviews[(int)toIndex];

					double fromIndex = y * horizontalSileces * 2f + (x * 2 + 1);
					UIView fromCheckboardSquareView = transitionContainer.Subviews[(int)fromIndex];

					CGVector sliceOriginVector;

					if (isPush)
						sliceOriginVector = new CGVector (fromCheckboardSquareView.Frame.GetMinX () - transitionContainer.Bounds.GetMinX (), fromCheckboardSquareView.Frame.GetMinY () - transitionContainer.Bounds.GetMinY ());
					else
						sliceOriginVector = new CGVector (fromCheckboardSquareView.Frame.GetMaxX () - transitionContainer.Bounds.GetMaxX (), fromCheckboardSquareView.Frame.GetMaxY () - transitionContainer.Bounds.GetMaxY ());

					// Project sliceOriginVector onto transitionVector.
					nfloat dot = sliceOriginVector.dx * transitionVector.dx + sliceOriginVector.dy * transitionVector.dy;
					var projection = new CGVector (transitionUnitVector.dx * dot / transitionVectorLength, transitionUnitVector.dy * dot / transitionVectorLength);

					// Compute the length of the projection.
					var projectionLength = NMath.Sqrt (projection.dx * projection.dx + projection.dy * projection.dy);

					double startTime = projectionLength / (transitionVectorLength + transitionSpacing) * transitionDuration;
					double duration = ((projectionLength + transitionSpacing) / (transitionVectorLength + transitionSpacing) * transitionDuration) - startTime;

					sliceAnimationsPending++;

					UIView.Animate(duration, startTime, UIViewAnimationOptions.TransitionNone, () => {
							toCheckboardSquareView.Layer.Transform = CATransform3D.Identity;
							fromCheckboardSquareView.Layer.Transform = CATransform3D.MakeRotation (NMath.PI, 0f, 1f, 0f);
						}, () => {
							if (--sliceAnimationsPending == 0) {
								bool wasCancelled = transitionContext.TransitionWasCancelled;
								transitionContainer.RemoveFromSuperview ();
								transitionContext.CompleteTransition (!wasCancelled);
							}
						}
					);
				}
			}
		}
	}
}
