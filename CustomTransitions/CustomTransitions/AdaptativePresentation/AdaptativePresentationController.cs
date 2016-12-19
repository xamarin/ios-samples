using UIKit;
using Foundation;
using CoreGraphics;

namespace CustomTransitions
{
	public class AdaptativePresentationController : UIPresentationController, IUIViewControllerTransitioningDelegate, IUIViewControllerAnimatedTransitioning
	{
		public UIButton dismissButton;
		public UIView presentationWrappingView;

		public AdaptativePresentationController(UIViewController presentedViewController, UIViewController presentingViewController)
			: base (presentedViewController, presentingViewController)
		{
			presentedViewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
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

			{
				UIView presentationWrapperView = new UIView(new CGRect(0f, 0f, 0f, 0f));
				presentationWrapperView.Layer.ShadowOpacity = 0.63F;
				presentationWrapperView.Layer.ShadowRadius = 17.0F;
				presentationWrappingView = presentationWrapperView;

				presentedViewControllerView.AutoresizingMask = (UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight);
				presentationWrapperView.AddSubview(presentedViewControllerView);

				UIButton dismissButtonAux = new UIButton(UIButtonType.Custom);
				dismissButtonAux.Frame = new CGRect(0F, 0F, 26F, 26F);
				dismissButtonAux.SetImage(new UIImage("CloseButton"), UIControlState.Normal);
				dismissButtonAux.AddTarget((object sender, System.EventArgs e) => DismissButtonTapped(dismissButtonAux), UIControlEvent.TouchUpInside);
				this.dismissButton = dismissButtonAux;

				presentationWrapperView.AddSubview(dismissButton);
			}
		}

		public void DismissButtonTapped(UIButton sender) {
			PresentingViewController.DismissViewController(true, null);
		}

		public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize(toSize, coordinator);

			presentationWrappingView.ClipsToBounds = true;
			presentationWrappingView.Layer.ShadowOpacity = .0F;
			presentationWrappingView.Layer.ShadowRadius = .0F;

			coordinator.AnimateAlongsideTransition((IUIViewControllerTransitionCoordinatorContext obj) => 
			{ 
			
			}, (IUIViewControllerTransitionCoordinatorContext obj) =>
			{
				presentationWrappingView.ClipsToBounds = false;
				presentationWrappingView.Layer.ShadowOpacity = 0.63F;
				presentationWrappingView.Layer.ShadowRadius = 17F;
			});
		}

		public override CGSize GetSizeForChildContentContainer(IUIContentContainer contentContainer, CGSize parentContainerSize) {
			if (contentContainer == PresentedViewController) {
				return new CGSize(parentContainerSize.Width / 2F, parentContainerSize.Height / 2F);
			}

			return base.GetSizeForChildContentContainer(contentContainer, parentContainerSize);
		}

		public override CGRect FrameOfPresentedViewInContainerView{
			get {
				CGRect containerViewBounds = ContainerView.Bounds;
				CGSize presentedViewContentSize = GetSizeForChildContentContainer(PresentedViewController, containerViewBounds.Size);

				CGRect frame = new CGRect(containerViewBounds.GetMidX() - (presentedViewContentSize.Width / 2F), 
				                          containerViewBounds.GetMidY() - (presentedViewContentSize.Height/2), 
				                          presentedViewContentSize.Width, presentedViewContentSize.Height);

				return frame.Inset(-20F, -20F);
			}
		}

		public override void ContainerViewWillLayoutSubviews()
		{
			base.ContainerViewWillLayoutSubviews();

			presentationWrappingView.Frame = FrameOfPresentedViewInContainerView;

			PresentedViewController.View.Frame = presentationWrappingView.Bounds.Inset(20F, 20F);

			dismissButton.Center = new CGPoint(PresentedViewController.View.Frame.GetMinX(), PresentedViewController.View.Frame.GetMinY());
		}



		public double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
		{
			if (transitionContext.IsAnimated)
			{
				return 0.35;
			}
			return 0;
		}

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

			if (toView != null)
			{
				containerView.AddSubview(toView);
			}

			if (isPresenting)
			{
				toView.Alpha = 0F;

				if (fromView != null)
				{
					fromView.Frame = transitionContext.GetFinalFrameForViewController(fromViewController);
				}
				toView.Frame = transitionContext.GetFinalFrameForViewController(toViewController);
			}
			else {
				if (toView != null)
				{
					toView.Frame = transitionContext.GetFinalFrameForViewController(toViewController);
				}
			}

			double transitionDuration = TransitionDuration(transitionContext);

			UIView.Animate(transitionDuration, 0, UIViewAnimationOptions.TransitionNone,
				() =>
				{
					if (isPresenting)
					{
						toView.Alpha = 1F;
					}
					else {
						fromView.Alpha = 0F;
					}
				},
				() =>
				{
					bool wasCancelled = transitionContext.TransitionWasCancelled;
					transitionContext.CompleteTransition(!wasCancelled);

					if (isPresenting == false) {
						fromView.Alpha = 1F;	
					}
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
