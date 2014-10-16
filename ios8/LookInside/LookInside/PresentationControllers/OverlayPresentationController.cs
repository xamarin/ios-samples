using System;

using UIKit;
using CoreGraphics;

namespace LookInside
{
	public class OverlayPresentationController : UIPresentationController
	{
		UIView dimmingView;
		UITapGestureRecognizer tap;

		public override UIModalPresentationStyle PresentationStyle {
			get {
				return UIModalPresentationStyle.OverFullScreen;
			}
		}

		public override bool ShouldPresentInFullscreen {
			get {
				return true;
			}
		}

		public override CGRect FrameOfPresentedViewInContainerView {
			get {
				CGRect presentedViewFrame = CGRect.Empty;
				CGRect containerBounds = ContainerView.Bounds;

				presentedViewFrame.Size = GetSizeForChildContentContainer (PresentedViewController, containerBounds.Size);
				nfloat newX = containerBounds.Size.Width - presentedViewFrame.Size.Width;
				presentedViewFrame.Location = new CGPoint (newX, presentedViewFrame.Location.Y);

				return presentedViewFrame;
			}
		}

		public OverlayPresentationController (UIViewController presentedViewController,
			                                  UIViewController presentingViewController)
			: base(presentedViewController, presentingViewController)
		{
			PrepareDimmingView ();
		}

		public override void PresentationTransitionWillBegin ()
		{
			base.PresentationTransitionWillBegin ();

			dimmingView.Frame = ContainerView.Bounds;
			dimmingView.Alpha = 0;

			ContainerView.InsertSubview (dimmingView, 0);
			SetAplhaOnTransitionBegin (1);
		}

		public override void DismissalTransitionWillBegin ()
		{
			base.DismissalTransitionWillBegin ();
			SetAplhaOnTransitionBegin (0);
		}

		void SetAplhaOnTransitionBegin(float alpha)
		{
			var transitionCoordinator = PresentedViewController.GetTransitionCoordinator ();
			if(transitionCoordinator == null)
			{
				dimmingView.Alpha = alpha;
				return;
			}

			transitionCoordinator.AnimateAlongsideTransition (context => {
				dimmingView.Alpha = alpha;
			}, null);
		}

		public override UIModalPresentationStyle AdaptivePresentationStyle ()
		{
			return UIModalPresentationStyle.OverFullScreen;
		}

		public override CGSize GetSizeForChildContentContainer (IUIContentContainer contentContainer, CGSize parentContainerSize)
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				return parentContainerSize;
			else
				return new CGSize(parentContainerSize.Width / 3, parentContainerSize.Height);
		}

		public override void ContainerViewWillLayoutSubviews ()
		{
			base.ContainerViewWillLayoutSubviews ();

			dimmingView.Frame = ContainerView.Bounds;
			PresentedView.Frame = FrameOfPresentedViewInContainerView;
		}

		void PrepareDimmingView()
		{
			dimmingView = new UIView ();
			dimmingView.BackgroundColor = UIColor.FromWhiteAlpha (0, 0.4f);
			dimmingView.Alpha = 0;

			tap = new UITapGestureRecognizer (DimmingViewTapped);
			dimmingView.AddGestureRecognizer (tap);
		}

		void DimmingViewTapped(UIGestureRecognizer gesture)
		{
			if (gesture.State == UIGestureRecognizerState.Recognized)
				PresentingViewController.DismissViewController (true, null);
		}
	}
}

