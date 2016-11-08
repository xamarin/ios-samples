using UIKit;
using Foundation;
using CoreGraphics;

namespace CustomTransitions
{
	public class CustomPresentationController : UIPresentationController, IUIViewControllerTransitioningDelegate, IUIViewControllerAnimatedTransitioning
	{
		public UIView dimmingView;
		public UIView presentationWrappingView;

		float cornerRadius;

		public CustomPresentationController(UIViewController presentedViewController, UIViewController presentingViewController)
			: base (presentedViewController, presentingViewController)
		{
			presentedViewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
			cornerRadius = 16F;
		}


		public override UIView PresentedView
		{
			get
			{
				return presentationWrappingView;
			}
		}


		public override void PresentationTransitionWillBegin()
		{
			UIView presentedViewControllerView = base.PresentedView;

			//	if (presentedViewController == null) {
			//	presentedViewController = PresentedViewController.View;
			//	return;
			//}

			{
				UIView presentationWrapperView = new UIView(this.FrameOfPresentedViewInContainerView);
				presentationWrapperView.Layer.ShadowOpacity = 0.44F;
				presentationWrapperView.Layer.ShadowRadius = 13.0F;
				presentationWrapperView.Layer.ShadowOffset = new CGSize(0.0F, -6.0F);
				this.presentationWrappingView = presentationWrapperView;

				UIView presentationRoundedCornerView = new UIView(UIEdgeInsetsInsetRect(presentationWrapperView.Bounds, new UIEdgeInsets(0F, 0F, -cornerRadius, 0)));
				presentationRoundedCornerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
				presentationRoundedCornerView.Layer.CornerRadius = cornerRadius;
				presentationRoundedCornerView.Layer.MasksToBounds = true;



				UIView presentedViewControllerWrapperView = new UIView(UIEdgeInsetsInsetRect(presentationRoundedCornerView.Bounds, new UIEdgeInsets(0F, 0F, cornerRadius, 0F)));
				presentedViewControllerWrapperView.AutoresizingMask = (UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight);

				presentedViewControllerView.AutoresizingMask = (UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight);
				presentedViewControllerView.Frame = presentedViewControllerWrapperView.Bounds;
				presentedViewControllerWrapperView.AddSubview(presentedViewControllerView);

				presentationRoundedCornerView.AddSubview(presentedViewControllerWrapperView);
				presentationWrapperView.AddSubview(presentationRoundedCornerView);
			}

			{
				UIView dimmingViewAux = new UIView(ContainerView.Frame);
				dimmingViewAux.BackgroundColor = UIColor.Black;
				dimmingViewAux.Opaque = false;
				dimmingViewAux.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
				dimmingView = dimmingViewAux;

				UITapGestureRecognizer tapGesture = new UITapGestureRecognizer();
				tapGesture.AddTarget(() => DimmingViewTapped(tapGesture));
				dimmingView.AddGestureRecognizer(tapGesture);

				ContainerView.AddSubview(dimmingView);

				IUIViewControllerTransitionCoordinator transitionCoordinator = PresentingViewController.GetTransitionCoordinator();

				dimmingView.Alpha = 0.0F;
				transitionCoordinator.AnimateAlongsideTransition((obj) => { dimmingView.Alpha = 0.5F; }, (obj) => {});
			}
		}


		CGRect UIEdgeInsetsInsetRect(CGRect rect, UIEdgeInsets insets)
		{
			rect.X += insets.Left;
			rect.Y += insets.Top;
			rect.Width -= (insets.Left + insets.Right);
			rect.Height -= (insets.Top + insets.Bottom);

			return rect;
		}


		public override void PresentationTransitionDidEnd(bool completed) {
			//base.PresentationTransitionDidEnd(completed);

			if (completed == false) {
				presentationWrappingView = null;
				dimmingView = null;
			}
		}


		public override void DismissalTransitionWillBegin() {
			IUIViewControllerTransitionCoordinator transitionCoordinator = PresentingViewController.GetTransitionCoordinator();
			transitionCoordinator.AnimateAlongsideTransition((obj) => { dimmingView.Alpha = 0.0F; }, (obj) => { });
		}


		public override void DismissalTransitionDidEnd(bool completed)
		{
			//base.PresentationTransitionDidEnd(completed);

			if (completed == true)
			{
				presentationWrappingView = null;
				dimmingView = null;
			}
		}



		// ------
		//[Export("preferredContentSizeDidChangeForChildContentContainer:")]
		public override void PreferredContentSizeDidChangeForChildContentContainer(IUIContentContainer container)
		{
			if (ContainerView != null && container == PresentedViewController) {
				ContainerView.SetNeedsLayout();
			}
		}

		public override CGSize GetSizeForChildContentContainer(IUIContentContainer contentContainer, CGSize parentContainerSize) {
			if (contentContainer == null) {
				return new CGSize(0F, 0F);
			}

			if (contentContainer == PresentedViewController)
			{
				return contentContainer.PreferredContentSize;
			}

			return base.GetSizeForChildContentContainer(contentContainer, parentContainerSize);
		}


		public override  CGRect FrameOfPresentedViewInContainerView
		{
			get {
				CGRect containerViewBounds = ContainerView.Bounds;
				CGSize presentedViewContentSize = this.GetSizeForChildContentContainer(this.PresentedViewController, containerViewBounds.Size);

				CGRect presentedViewControllerFrame = containerViewBounds;
				presentedViewControllerFrame.Height = presentedViewContentSize.Height;
				presentedViewControllerFrame.Y = containerViewBounds.GetMaxY() - presentedViewContentSize.Height;

				// OJO ACA
				return presentedViewControllerFrame;
			}
		}



		// ------

		public override void ContainerViewWillLayoutSubviews() {
			base.ContainerViewWillLayoutSubviews();

			dimmingView.Frame = ContainerView.Bounds;
			presentationWrappingView.Frame = FrameOfPresentedViewInContainerView;
		}

		// ------




		void DimmingViewTapped(UITapGestureRecognizer sender)
		{
			PresentingViewController.DismissViewController(true, null);
		}



		// ------


		public double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
		{
			if (transitionContext.IsAnimated)
			{
				return 0.35;
			}
			return 0;
		}

		//| ----------------------------------------------------------------------------
		//  The presentation animation is tightly integrated with the overall
		//  presentation so it makes the most sense to implement
		//  <UIViewControllerAnimatedTransitioning> in the presentation controller
		//  rather than in a separate object.
		//
		public void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
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
			UIView toView = transitionContext.GetViewFor(UITransitionContext.ToViewKey);
			UIView fromView = transitionContext.GetViewFor(UITransitionContext.FromViewKey);

			bool isPresenting = (fromViewController == PresentingViewController);

			CGRect fromViewFinalFrame = transitionContext.GetFinalFrameForViewController(fromViewController);
			CGRect toViewInitialFrame = transitionContext.GetInitialFrameForViewController(toViewController);

			CGRect toViewFinalFrame = transitionContext.GetFinalFrameForViewController(toViewController);

			if (toView != null)
			{
				containerView.AddSubview(toView);
			}
			if (isPresenting)
			{
				toViewInitialFrame.X = containerView.Bounds.GetMinX();
				toViewInitialFrame.Y = containerView.Bounds.GetMaxY();

				toViewInitialFrame.Size = toViewFinalFrame.Size;

				toView.Frame = toViewInitialFrame;
			}
			else {
				fromView.Frame.Offset(0F, fromView.Frame.Height);

				fromViewFinalFrame = new CGRect(fromView.Frame.X, fromView.Frame.Y + fromView.Frame.Height, fromViewFinalFrame.Width, fromViewFinalFrame.Height);
			}

			double transitionDuration = TransitionDuration(transitionContext);

			UIView.Animate(transitionDuration, 0, UIViewAnimationOptions.TransitionNone,
				() =>
				{
					if (isPresenting)
					{
						toView.Frame = toViewFinalFrame;
					}	
					else {
						fromView.Frame = fromViewFinalFrame;
					}
				},
				() =>
				{
					bool wasCancelled = transitionContext.TransitionWasCancelled;
					transitionContext.CompleteTransition(!wasCancelled);
				}
			);
		}


		//  If the modalPresentationStyle of the presented view controller is
		//  UIModalPresentationCustom, the system calls this method on the presented
		//  view controller's transitioningDelegate to retrieve the presentation
		//  controller that will manage the presentation.  If your implementation
		//  returns nil, an instance of UIPresentationController is used.
		//
		[Export("presentationControllerForPresentedViewController:presentingViewController:sourceViewController:")]
		public UIPresentationController GetPresentationControllerForPresentedViewController(UIViewController presentedViewController, UIViewController presentingViewController, UIViewController sourceViewController)
		{
			return this;
		}


		//| ----------------------------------------------------------------------------
		//  The system calls this method on the presented view controller's
		//  transitioningDelegate to retrieve the animator object used for animating
		//  the dismissal of the presented view controller.  Your implementation is
		//  expected to return an object that conforms to the
		//  UIViewControllerAnimatedTransitioning protocol, or nil if the default
		//  dismissal animation should be used.
		//
		[Export("animationControllerForPresentedController:presentingController:sourceController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
		{
			return this;
		}


		//| ----------------------------------------------------------------------------
		//  The system calls this method on the presented view controller's
		//  transitioningDelegate to retrieve the animator object used for animating
		//  the dismissal of the presented view controller.  Your implementation is
		//  expected to return an object that conforms to the
		//  UIViewControllerAnimatedTransitioning protocol, or nil if the default
		//  dismissal animation should be used.
		//
		[Export("animationControllerForDismissedController:")]
		public IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
		{
			return this;
		}
	}
}
