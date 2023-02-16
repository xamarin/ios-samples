using UIKit;
using Foundation;
using CoreGraphics;

namespace CustomTransitions {
	public class AdaptivePresentationController : UIPresentationController, IUIViewControllerTransitioningDelegate, IUIViewControllerAnimatedTransitioning {

		UIButton dismissButton;
		UIView presentationWrappingView;

		public AdaptivePresentationController (UIViewController presentedViewController, UIViewController presentingViewController) : base (presentedViewController, presentingViewController)
		{
			presentedViewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
		}

		public override UIView PresentedView {
			get {
				return presentationWrappingView;
			}
		}

		public override void PresentationTransitionWillBegin ()
		{
			UIView presentedViewControllerView = base.PresentedView;

			var presentationWrapperView = new UIView (new CGRect (0f, 0f, 0f, 0f));
			presentationWrapperView.Layer.ShadowOpacity = .63f;
			presentationWrapperView.Layer.ShadowRadius = 17f;
			presentationWrappingView = presentationWrapperView;

			presentedViewControllerView.AutoresizingMask = (UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight);
			presentationWrapperView.AddSubview (presentedViewControllerView);

			var dismissButtonAux = new UIButton (UIButtonType.Custom) {
				Frame = new CGRect (0f, 0f, 26f, 26f)
			};
			dismissButtonAux.SetImage (new UIImage ("CloseButton"), UIControlState.Normal);
			dismissButtonAux.AddTarget ((object sender, System.EventArgs e) => DismissButtonTapped (dismissButtonAux), UIControlEvent.TouchUpInside);

			dismissButton = dismissButtonAux;
			presentationWrapperView.AddSubview (dismissButton);
		}

		public void DismissButtonTapped (UIButton sender)
		{
			PresentingViewController.DismissViewController (true, null);
		}

		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (toSize, coordinator);

			presentationWrappingView.ClipsToBounds = true;
			presentationWrappingView.Layer.ShadowOpacity = .0f;
			presentationWrappingView.Layer.ShadowRadius = .0f;

			coordinator.AnimateAlongsideTransition ((obj) => { }, (obj) => {
				presentationWrappingView.ClipsToBounds = false;
				presentationWrappingView.Layer.ShadowOpacity = .63f;
				presentationWrappingView.Layer.ShadowRadius = 17f;
			});
		}

		public override CGSize GetSizeForChildContentContainer (IUIContentContainer contentContainer, CGSize parentContainerSize)
		{
			if (contentContainer == PresentedViewController)
				return new CGSize (parentContainerSize.Width / 2f, parentContainerSize.Height / 2f);

			return base.GetSizeForChildContentContainer (contentContainer, parentContainerSize);
		}

		public override CGRect FrameOfPresentedViewInContainerView {
			get {
				CGRect containerViewBounds = ContainerView.Bounds;
				var presentedViewContentSize = GetSizeForChildContentContainer (PresentedViewController, containerViewBounds.Size);

				var frame = new CGRect (containerViewBounds.GetMidX () - (presentedViewContentSize.Width / 2f),
										  containerViewBounds.GetMidY () - (presentedViewContentSize.Height / 2f),
										  presentedViewContentSize.Width, presentedViewContentSize.Height);

				return frame.Inset (-20f, -20f);
			}
		}

		public override void ContainerViewWillLayoutSubviews ()
		{
			base.ContainerViewWillLayoutSubviews ();

			presentationWrappingView.Frame = FrameOfPresentedViewInContainerView;
			PresentedViewController.View.Frame = presentationWrappingView.Bounds.Inset (20f, 20f);
			dismissButton.Center = new CGPoint (PresentedViewController.View.Frame.GetMinX (), PresentedViewController.View.Frame.GetMinY ());
		}

		public double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return transitionContext.IsAnimated ? 0 : 0.35;
		}

		public void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			var fromViewController = transitionContext.GetViewControllerForKey (UITransitionContext.FromViewControllerKey);
			var toViewController = transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);
			UIView containerView = transitionContext.ContainerView;

			var toView = transitionContext.GetViewFor (UITransitionContext.ToViewKey);
			var fromView = transitionContext.GetViewFor (UITransitionContext.FromViewKey);

			bool isPresenting = (fromViewController == PresentingViewController);

			if (toView != null)
				containerView.AddSubview (toView);

			if (isPresenting) {
				toView.Alpha = 0f;

				if (fromView != null)
					fromView.Frame = transitionContext.GetFinalFrameForViewController (fromViewController);

				toView.Frame = transitionContext.GetFinalFrameForViewController (toViewController);
			} else if (toView != null) {
				toView.Frame = transitionContext.GetFinalFrameForViewController (toViewController);
			}

			double transitionDuration = TransitionDuration (transitionContext);

			UIView.Animate (transitionDuration, 0, UIViewAnimationOptions.TransitionNone, () => {
				if (toView != null)
					toView.Alpha = isPresenting ? 1f : 0f;
			}, () => {
				bool wasCancelled = transitionContext.TransitionWasCancelled;
				transitionContext.CompleteTransition (!wasCancelled);

				if (isPresenting == false)
					fromView.Alpha = 1f;
			}
			);
		}

		[Export ("presentationControllerForPresentedViewController:presentingViewController:sourceViewController:")]
		public UIPresentationController GetPresentationControllerForPresentedViewController (UIViewController presentedViewController, UIViewController presentingViewController, UIViewController sourceViewController)
		{
			return this;
		}

		[Export ("animationControllerForPresentedController:presentingController:sourceController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source)
		{
			return this;
		}

		[Export ("animationControllerForDismissedController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController (UIViewController dismissed)
		{
			return this;
		}
	}
}
