using UIKit;
using System;
using CoreGraphics;

namespace CustomTransitions
{
	public class CheckboardTransitionAnimator : UIViewControllerAnimatedTransitioning
	{

		public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
		{
			return 3.0;
		}

		public override void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
		{
			UIViewController fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
			UIViewController toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

			UIView containerView = transitionContext.ContainerView;

			UIView fromView = transitionContext.GetViewFor(UITransitionContext.FromViewKey);
			UIView toView = transitionContext.GetViewFor(UITransitionContext.ToViewKey);

			fromView.Frame = transitionContext.GetInitialFrameForViewController(fromViewController);
			toView.Frame = transitionContext.GetFinalFrameForViewController(toViewController);

			bool isPush = Array.IndexOf(toViewController.NavigationController.ViewControllers, toViewController) > Array.IndexOf(fromViewController.NavigationController.ViewControllers, fromViewController);

			containerView.AddSubview(toView);

			UIImage fromViewSnapshot;
			UIImage toViewSnapshot = new UIImage();
				
			UIGraphics.BeginImageContextWithOptions(containerView.Bounds.Size, true, containerView.Window.Screen.Scale);
			fromView.DrawViewHierarchy(containerView.Bounds, false);
			fromViewSnapshot = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() => { 
				UIGraphics.BeginImageContextWithOptions(containerView.Bounds.Size, true, containerView.Window.Screen.Scale);
				toView.DrawViewHierarchy(containerView.Bounds, false);
				toViewSnapshot = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
			});

			UIView transitionContainer = new UIView(containerView.Bounds);
			transitionContainer.Opaque = true;
			transitionContainer.BackgroundColor = UIColor.Black;
			containerView.AddSubview(transitionContainer);

		    // Apply a perpective transform to the sublayers of transitionContainer.
			CoreAnimation.CATransform3D t = CoreAnimation.CATransform3D.Identity;
			t.m34 = 1.0F / -900.0F;
			transitionContainer.Layer.SublayerTransform = t;

			//// The size and number of slices is a function of the width.
			double sliceSize = Math.Round(transitionContainer.Frame.Width / 10.0F);
			double horizontalSileces = Math.Ceiling(transitionContainer.Frame.Width / sliceSize);
			double verticalSlices = Math.Ceiling(transitionContainer.Frame.Height / sliceSize);

			//// transitionSpacing controls the transition duration for each slice.
			//// Higher values produce longer animations with multiple slices having
			//// their animations 'in flight' simultaneously.
			float transitionSpacing = 160.0F;
			double transitionDuration = this.TransitionDuration(transitionContext);

			CGVector transitionVector;
			if (isPush)
			{
				transitionVector = new CGVector(transitionContainer.Bounds.GetMaxY() - transitionContainer.Bounds.GetMinX(), transitionContainer.Bounds.GetMaxY() - transitionContainer.Bounds.GetMinY());
			}
			else { 
				transitionVector = new CGVector(transitionContainer.Bounds.GetMinX() - transitionContainer.Bounds.GetMaxX(), transitionContainer.Bounds.GetMinY() - transitionContainer.Bounds.GetMaxY());
			}

			double transitionVectorLength = Math.Sqrt(transitionVector.dx * transitionVector.dx + transitionVector.dy * transitionVector.dy);
			CGVector transitionUnitVector = new CGVector(transitionVector.dx / new nfloat(transitionVectorLength), transitionVector.dy / new nfloat(transitionVectorLength));

			for (int y = 0; y < verticalSlices; y++)
			{
				for (int x = 0; x < horizontalSileces; x++)
				{
					CoreAnimation.CALayer fromContentLayer = new CoreAnimation.CALayer();
					fromContentLayer.Frame = new CGRect(x + sliceSize * -1.0F, y * sliceSize * -1F, containerView.Bounds.Width, containerView.Bounds.Height);
					fromContentLayer.RasterizationScale = fromViewSnapshot.CurrentScale;
					fromContentLayer.Contents = fromViewSnapshot.CGImage;

					CoreAnimation.CALayer toContentLayer = new CoreAnimation.CALayer();
					toContentLayer.Frame = new CGRect(x * sliceSize * -1.0F, y * sliceSize * -1F, containerView.Bounds.Width, containerView.Bounds.Height);

					CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() => {
						bool wereActionDisabled = CoreAnimation.CATransaction.DisableActions;
						CoreAnimation.CATransaction.DisableActions = true;

						toContentLayer.RasterizationScale = toViewSnapshot.CurrentScale;
						toContentLayer.Contents = toViewSnapshot.CGImage;

						CoreAnimation.CATransaction.DisableActions = wereActionDisabled;
					});

					UIView toCheckboardSquareView = new UIView();
					toCheckboardSquareView.Frame = new CGRect(x * sliceSize, y * sliceSize, sliceSize, sliceSize);
					toCheckboardSquareView.Opaque = false;
					toCheckboardSquareView.Layer.MasksToBounds = true;
					toCheckboardSquareView.Layer.DoubleSided = false;
					toCheckboardSquareView.Layer.Transform = CoreAnimation.CATransform3D.MakeRotation(new nfloat(Math.PI), 0, 1, 0);
					toCheckboardSquareView.Layer.AddSublayer(toContentLayer);

					UIView fromCheckboardSquareView = new UIView();
					fromCheckboardSquareView.Frame = new CGRect(x * sliceSize, y * sliceSize, sliceSize, sliceSize);
					fromCheckboardSquareView.Opaque = false;
					fromCheckboardSquareView.Layer.MasksToBounds = true;
					fromCheckboardSquareView.Layer.DoubleSided = false;
					fromCheckboardSquareView.Layer.Transform = CoreAnimation.CATransform3D.Identity;
					fromCheckboardSquareView.Layer.AddSublayer(fromContentLayer);

					transitionContainer.AddSubview(toCheckboardSquareView);
					transitionContainer.AddSubview(fromCheckboardSquareView);
				}
			}

			int sliceAnimationsPending = 0;

			for (int y = 0; y < verticalSlices; y++)
			{
				for (int x = 0; x < horizontalSileces; x++)
				{ 
					int toIndex = (int)(y * horizontalSileces * 2F + (x * 2));
					UIView toCheckboardSquareView = transitionContainer.Subviews[toIndex];

					int fromIndex = (int)(y * horizontalSileces * 2F + (x * 2 + 1));
					UIView fromCheckboardSquareView = transitionContainer.Subviews[fromIndex];

					CGVector sliceOriginVector;

					if (isPush)
					{
						// Define a vector from the origin of transitionContainer to the
						// top left corner of the slice.
						sliceOriginVector = new CGVector(fromCheckboardSquareView.Frame.GetMinX() - transitionContainer.Bounds.GetMinX(), fromCheckboardSquareView.Frame.GetMinY() - transitionContainer.Bounds.GetMinY());
					}
					else { 
						// Define a vector from the bottom right corner of
						// transitionContainer to the bottom right corner of the slice.
						sliceOriginVector = new CGVector(fromCheckboardSquareView.Frame.GetMaxX() - transitionContainer.Bounds.GetMaxX(), fromCheckboardSquareView.Frame.GetMaxY() - transitionContainer.Bounds.GetMaxY());
					}


		            // Project sliceOriginVector onto transitionVector.
					double dot = sliceOriginVector.dx * transitionVector.dx + sliceOriginVector.dy * transitionVector.dy;
					CGVector projection = new CGVector(transitionUnitVector.dx * new nfloat(dot) / new nfloat(transitionVectorLength), transitionUnitVector.dy * new nfloat(dot) / new nfloat(transitionVectorLength));

		            // Compute the length of the projection.
					double projectionLength = Math.Sqrt(projection.dx * projection.dx + projection.dy * projection.dy);

					double startTime = projectionLength / (transitionVectorLength + transitionSpacing) * transitionDuration;
					double duration = ((projectionLength + transitionSpacing) / (transitionVectorLength + transitionSpacing) * transitionDuration) - startTime;

					sliceAnimationsPending++;


					UIView.Animate(duration, startTime, UIViewAnimationOptions.TransitionNone,() =>
						{
							toCheckboardSquareView.Layer.Transform = CoreAnimation.CATransform3D.Identity;
							fromCheckboardSquareView.Layer.Transform = CoreAnimation.CATransform3D.MakeRotation(new nfloat(Math.PI), 0, 1, 0);
						}, () =>
						{
							if (--sliceAnimationsPending == 0) {
								// Finish the transition once the final animation completes.
								bool wasCancelled = transitionContext.TransitionWasCancelled;
								transitionContainer.RemoveFromSuperview();

								// When we complete, tell the transition context
								// passing along the BOOL that indicates whether the transition
								// finished or not.
								transitionContext.CompleteTransition(!wasCancelled);
							}
						}
					);
					
				}
			}
		}



	}
}



