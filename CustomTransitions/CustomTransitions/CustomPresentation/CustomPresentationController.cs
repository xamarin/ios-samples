using CoreGraphics;
using Foundation;
using UIKit;

namespace CustomTransitions {
	public class CustomPresentationController : UIPresentationController, IUIViewControllerTransitioningDelegate, IUIViewControllerAnimatedTransitioning {
		UIView dimmingView;
		UIView presentationWrapperView;

		float cornerRadius;

		public override UIView PresentedView {
			get {
				return presentationWrapperView;
			}
		}

		public CustomPresentationController (UIViewController presentedViewController, UIViewController presentingViewController) : base (presentedViewController, presentingViewController)
		{
			presentedViewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
			cornerRadius = 16f;
		}

		public override void PresentationTransitionWillBegin ()
		{
			UIView presentedViewControllerView = base.PresentedView;

			presentationWrapperView = new UIView (FrameOfPresentedViewInContainerView);
			presentationWrapperView.Layer.ShadowOpacity = .44f;
			presentationWrapperView.Layer.ShadowRadius = 13f;
			presentationWrapperView.Layer.ShadowOffset = new CGSize (0f, -6f);

			var presentationRoundedCornerView = new UIView (UIEdgeInsetsInsetRect (presentationWrapperView.Bounds, new UIEdgeInsets (0F, 0F, -cornerRadius, 0)));
			presentationRoundedCornerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			presentationRoundedCornerView.Layer.CornerRadius = cornerRadius;
			presentationRoundedCornerView.Layer.MasksToBounds = true;

			var presentedViewControllerWrapperView = new UIView (UIEdgeInsetsInsetRect (presentationRoundedCornerView.Bounds, new UIEdgeInsets (0F, 0F, cornerRadius, 0F)));
			presentedViewControllerWrapperView.AutoresizingMask = (UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight);

			presentedViewControllerView.AutoresizingMask = (UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight);
			presentedViewControllerView.Frame = presentedViewControllerWrapperView.Bounds;
			presentedViewControllerWrapperView.AddSubview (presentedViewControllerView);

			presentationRoundedCornerView.AddSubview (presentedViewControllerWrapperView);
			presentationWrapperView.AddSubview (presentationRoundedCornerView);

			var dimmingViewAux = new UIView (ContainerView.Frame) {
				BackgroundColor = UIColor.Black,
				Opaque = false,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
			};

			dimmingView = dimmingViewAux;

			var tapGesture = new UITapGestureRecognizer ();
			tapGesture.AddTarget (() => DimmingViewTapped (tapGesture));
			dimmingView.AddGestureRecognizer (tapGesture);

			ContainerView.AddSubview (dimmingView);

			var transitionCoordinator = PresentingViewController.GetTransitionCoordinator ();

			dimmingView.Alpha = 0f;
			transitionCoordinator.AnimateAlongsideTransition ((obj) => dimmingView.Alpha = .5f, (obj) => { });
		}

		CGRect UIEdgeInsetsInsetRect (CGRect rect, UIEdgeInsets insets)
		{
			rect.X += insets.Left;
			rect.Y += insets.Top;
			rect.Width -= (insets.Left + insets.Right);
			rect.Height -= (insets.Top + insets.Bottom);

			return rect;
		}

		public override void PresentationTransitionDidEnd (bool completed)
		{
			if (completed)
				return;

			presentationWrapperView = null;
			dimmingView = null;
		}

		public override void DismissalTransitionWillBegin ()
		{
			var transitionCoordinator = PresentingViewController.GetTransitionCoordinator ();
			transitionCoordinator.AnimateAlongsideTransition ((obj) => dimmingView.Alpha = 0f, (obj) => { });
		}

		public override void DismissalTransitionDidEnd (bool completed)
		{
			if (!completed)
				return;

			presentationWrapperView = null;
			dimmingView = null;
		}

		public override void PreferredContentSizeDidChangeForChildContentContainer (IUIContentContainer container)
		{
			if (ContainerView != null && container == PresentedViewController)
				ContainerView.SetNeedsLayout ();
		}

		public override CGSize GetSizeForChildContentContainer (IUIContentContainer contentContainer, CGSize parentContainerSize)
		{
			if (contentContainer == null)
				return new CGSize (0f, 0f);

			if (contentContainer == PresentedViewController)
				return contentContainer.PreferredContentSize;

			return base.GetSizeForChildContentContainer (contentContainer, parentContainerSize);
		}

		public override CGRect FrameOfPresentedViewInContainerView {
			get {
				var containerViewBounds = ContainerView.Bounds;
				var presentedViewContentSize = GetSizeForChildContentContainer (PresentedViewController, containerViewBounds.Size);

				CGRect presentedViewControllerFrame = containerViewBounds;
				presentedViewControllerFrame.Height = presentedViewContentSize.Height;
				presentedViewControllerFrame.Y = containerViewBounds.GetMaxY () - presentedViewContentSize.Height;

				return presentedViewControllerFrame;
			}
		}

		public override void ContainerViewWillLayoutSubviews ()
		{
			base.ContainerViewWillLayoutSubviews ();

			dimmingView.Frame = ContainerView.Bounds;
			presentationWrapperView.Frame = FrameOfPresentedViewInContainerView;
		}

		void DimmingViewTapped (UITapGestureRecognizer sender)
		{
			PresentingViewController.DismissViewController (true, null);
		}

		public double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
		{
			return transitionContext.IsAnimated ? 0.35 : 0;
		}

		public void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
		{
			var fromViewController = transitionContext.GetViewControllerForKey (UITransitionContext.FromViewControllerKey);
			var toViewController = transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);

			UIView containerView = transitionContext.ContainerView;

			var toView = transitionContext.GetViewFor (UITransitionContext.ToViewKey);
			var fromView = transitionContext.GetViewFor (UITransitionContext.FromViewKey);
			bool isPresenting = (fromViewController == PresentingViewController);

			var fromViewFinalFrame = transitionContext.GetFinalFrameForViewController (fromViewController);
			var toViewInitialFrame = transitionContext.GetInitialFrameForViewController (toViewController);

			var toViewFinalFrame = transitionContext.GetFinalFrameForViewController (toViewController);

			if (toView != null)
				containerView.AddSubview (toView);

			if (isPresenting) {
				toViewInitialFrame.X = containerView.Bounds.GetMinX ();
				toViewInitialFrame.Y = containerView.Bounds.GetMaxY ();
				toViewInitialFrame.Size = toViewFinalFrame.Size;

				toView.Frame = toViewInitialFrame;
			} else {
				fromView.Frame.Offset (0f, fromView.Frame.Height);
				fromViewFinalFrame = new CGRect (fromView.Frame.X, fromView.Frame.Y + fromView.Frame.Height, fromViewFinalFrame.Width, fromViewFinalFrame.Height);
			}

			double transitionDuration = TransitionDuration (transitionContext);

			UIView.Animate (transitionDuration, 0, UIViewAnimationOptions.TransitionNone, () => {
				if (isPresenting)
					toView.Frame = toViewFinalFrame;
				else
					fromView.Frame = fromViewFinalFrame;
			}, () => {
				bool wasCancelled = transitionContext.TransitionWasCancelled;
				transitionContext.CompleteTransition (!wasCancelled);
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
