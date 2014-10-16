using System;

using UIKit;
using CoreGraphics;

namespace LookInside
{
	public class CoolPresentationController : UIPresentationController
	{
		UIImageView bigFlowerImageView;
		UIImageView carlImageView;

		UIImage jaguarPrintImageH;
		UIImage jaguarPrintImageV;

		UIImageView topJaguarPrintImageView;
		UIImageView bottomJaguarPrintImageView;

		UIImageView leftJaguarPrintImageView;
		UIImageView rightJaguarPrintImageView;

		UIView dimmingView;

		public override CGRect FrameOfPresentedViewInContainerView {
			get {
				CGRect containerBounds = ContainerView.Bounds;

				CGRect presentedViewFrame = CGRect.Empty;
				nfloat w = 300;
				nfloat h = TraitCollection.VerticalSizeClass == UIUserInterfaceSizeClass.Compact
					? 300
					: containerBounds.Size.Height - 2 * jaguarPrintImageH.Size.Height;
				presentedViewFrame.Size = new CGSize (w, h);
				var location = (CGPoint)containerBounds.Size;
				location.X /= 2;
				location.Y /= 2;
				location.X -= presentedViewFrame.Size.Width / 2;
				location.Y -= presentedViewFrame.Size.Height / 2;
				presentedViewFrame.Location = location;

				return presentedViewFrame;
			}
		}

		public CoolPresentationController (UIViewController presentedViewController,
			                               UIViewController presentingViewController)
			: base(presentedViewController, presentingViewController)
		{
			dimmingView = new UIView {
				BackgroundColor = UIColor.Purple.ColorWithAlpha(0.4f)
			};

			bigFlowerImageView = new UIImageView (UIImage.FromBundle ("BigFlower"));
			carlImageView = new UIImageView (UIImage.FromBundle ("Carl")) {
				Frame = new CGRect (0, 0, 500, 245)
			};

			jaguarPrintImageH = UIImage.FromBundle ("JaguarH").CreateResizableImage (UIEdgeInsets.Zero, UIImageResizingMode.Tile);
			jaguarPrintImageV = UIImage.FromBundle ("JaguarV").CreateResizableImage (UIEdgeInsets.Zero, UIImageResizingMode.Tile);

			topJaguarPrintImageView = new UIImageView (jaguarPrintImageH);
			bottomJaguarPrintImageView = new UIImageView (jaguarPrintImageH);

			leftJaguarPrintImageView = new UIImageView (jaguarPrintImageV);
			rightJaguarPrintImageView = new UIImageView (jaguarPrintImageV);
		}

		public override void PresentationTransitionWillBegin ()
		{
			base.PresentationTransitionWillBegin ();

			AddViewsToDimmingView ();

			dimmingView.Alpha = 0;

			PresentedViewController.GetTransitionCoordinator ().AnimateAlongsideTransition (context => {
				dimmingView.Alpha = 1;
			}, null);

			MoveJaguarPrintToPresentedPosition (false);

			UIView.Animate (1, () => {
				MoveJaguarPrintToPresentedPosition(true);
			});
		}

		public override void ContainerViewWillLayoutSubviews ()
		{
			base.ContainerViewWillLayoutSubviews ();
			dimmingView.Frame = ContainerView.Bounds;
			PresentedView.Frame = FrameOfPresentedViewInContainerView;
			MoveJaguarPrintToPresentedPosition (true);
		}

		public override void ContainerViewDidLayoutSubviews ()
		{
			base.ContainerViewDidLayoutSubviews ();
			CGPoint bigFlowerCenter = dimmingView.Frame.Location;
			bigFlowerCenter.X += bigFlowerImageView.Image.Size.Width / 4;
			bigFlowerCenter.Y += bigFlowerImageView.Image.Size.Height / 4;

			bigFlowerImageView.Center = bigFlowerCenter;

			CGRect carlFrame = carlImageView.Frame;
			CGPoint location = carlFrame.Location;
			location.Y = dimmingView.Bounds.Size.Height - carlFrame.Size.Height;

			carlImageView.Frame = carlFrame;
		}

		public override void DismissalTransitionWillBegin ()
		{
			base.DismissalTransitionWillBegin ();

			PresentedViewController.GetTransitionCoordinator().AnimateAlongsideTransition(context => {
				dimmingView.Alpha = 0;
			}, null);
		}

		public override void DismissalTransitionDidEnd (bool completed)
		{
			base.DismissalTransitionDidEnd (completed);
			Dispose ();
		}

		void AddViewsToDimmingView()
		{
			if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular &&
			   TraitCollection.VerticalSizeClass == UIUserInterfaceSizeClass.Regular) {
				dimmingView.AddSubviews (bigFlowerImageView, carlImageView);
			}

			dimmingView.AddSubviews (topJaguarPrintImageView,
				bottomJaguarPrintImageView, leftJaguarPrintImageView, rightJaguarPrintImageView);

			ContainerView.AddSubview (dimmingView);
		}

		void MoveJaguarPrintToPresentedPosition(bool presentedPosition)
		{
			CGSize horizontalJaguarSize = jaguarPrintImageH.Size;
			CGSize verticalJaguarSize = jaguarPrintImageV.Size;
			CGRect frameOfView = FrameOfPresentedViewInContainerView;
			CGRect containerFrame = ContainerView.Frame;

			CGRect topFrame, bottomFrame, leftFrame, rightFrame;
			topFrame = bottomFrame = leftFrame = rightFrame = CGRect.Empty;
			topFrame.Height = bottomFrame.Height = horizontalJaguarSize.Height;
			topFrame.Width = bottomFrame.Width = frameOfView.Width;

			leftFrame.Width = rightFrame.Width = verticalJaguarSize.Width;
			leftFrame.Height = rightFrame.Height = frameOfView.Height;

			topFrame.X = frameOfView.X;
			bottomFrame.X = frameOfView.X;

			leftFrame.Y = frameOfView.Y;
			rightFrame.Y = frameOfView.Y;

			CGRect frameToAlignAround = presentedPosition ? frameOfView : containerFrame;

			topFrame.Y = frameToAlignAround.GetMinY() - horizontalJaguarSize.Height;
			bottomFrame.Y = frameToAlignAround.GetMaxY ();
			leftFrame.X = frameToAlignAround.GetMinX () - verticalJaguarSize.Width;
			rightFrame.X = frameToAlignAround.GetMaxX ();

			topJaguarPrintImageView.Frame = topFrame;
			bottomJaguarPrintImageView.Frame = bottomFrame;
			leftJaguarPrintImageView.Frame = leftFrame;
			rightJaguarPrintImageView.Frame = rightFrame;
		}

		protected override void Dispose (bool disposing)
		{
			bigFlowerImageView.Dispose ();
			bigFlowerImageView = null;

			carlImageView.Dispose();
			carlImageView = null;

			jaguarPrintImageH.Dispose();
			jaguarPrintImageH = null;

			jaguarPrintImageV.Dispose();
			jaguarPrintImageV = null;

			topJaguarPrintImageView.Dispose ();
			topJaguarPrintImageView = null;

			bottomJaguarPrintImageView.Dispose ();
			bottomJaguarPrintImageView = null;

			leftJaguarPrintImageView.Dispose ();
			leftJaguarPrintImageView = null;

			rightJaguarPrintImageView.Dispose ();
			rightJaguarPrintImageView = null;

			dimmingView.Dispose ();
			dimmingView = null;

			base.Dispose (disposing);
		}
	}

}

