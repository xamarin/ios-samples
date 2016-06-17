using System;

using Foundation;
using CoreGraphics;
using UIKit;

namespace ZoomingPdfViewer {
	public partial class RootViewController : UIViewController, IUIPageViewControllerDelegate {
		UIPageViewController pageViewController;
		ModelController modelController;

		public RootViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			modelController = new ModelController ();

			pageViewController = new UIPageViewController (
				UIPageViewControllerTransitionStyle.PageCurl,
				UIPageViewControllerNavigationOrientation.Horizontal,
				new NSDictionary ()
			);

			pageViewController.Delegate = this;
			DataViewController startingViewController = modelController.GetViewController (0, Storyboard);
			pageViewController.SetViewControllers (
				new [] { startingViewController },
				UIPageViewControllerNavigationDirection.Forward,
				false, null
			);

			pageViewController.DataSource = modelController;
			AddChildViewController (pageViewController);
			View.AddSubview (pageViewController.View);

			CGRect pageViewRect = View.Bounds;

			pageViewController.View.Frame = pageViewRect;
			pageViewController.DidMoveToParentViewController (this);
			View.GestureRecognizers = pageViewController.GestureRecognizers;
		}

		[Export ("pageViewController:spineLocationForInterfaceOrientation:")]
		public UIPageViewControllerSpineLocation GetSpineLocation (UIPageViewController pageViewController, UIInterfaceOrientation orientation)
		{
			if (orientation == UIInterfaceOrientation.Portrait || UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				UIViewController currentViewController = pageViewController.ViewControllers [0];
				pageViewController.SetViewControllers (
					new [] { currentViewController },
					UIPageViewControllerNavigationDirection.Forward,
					true, null
				);

				pageViewController.DoubleSided = false;
				return UIPageViewControllerSpineLocation.Min;
			}

			var dataViewController = (DataViewController)pageViewController.ViewControllers [0];
			var indexOfCurrentViewController = modelController.IndexOfViewController (dataViewController);
			UIViewController[] viewControllers = null;

			if (indexOfCurrentViewController == 0 || indexOfCurrentViewController % 2 == 0) {
				UIViewController nextViewController = modelController.GetNextViewController (pageViewController, dataViewController);
				viewControllers = new [] { dataViewController, nextViewController };
			} else {
				UIViewController previousViewController = modelController.GetPreviousViewController (pageViewController, dataViewController);
				viewControllers = new[] { dataViewController, previousViewController };
			}

			pageViewController.SetViewControllers (viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);
			return UIPageViewControllerSpineLocation.Mid;
		}
	}
}
