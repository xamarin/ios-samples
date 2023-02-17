using System;

using Foundation;
using UIKit;

using static UIKit.UIPageViewControllerNavigationDirection;

namespace Flags {
	public partial class RootViewController : UIViewController {
		int currentIndex;
		UIPageViewController pageViewController;

		RootViewControllerDataSource dataSource;
		RootViewControllerDataSource DataSource {
			get {
				dataSource = dataSource ?? new RootViewControllerDataSource (Storyboard);
				return dataSource;
			}
		}

		public RootViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			pageViewController = ChildViewControllers [0] as UIPageViewController;
			if (pageViewController == null)
				return;

			var startingViewController = DataSource.GetViewControllerAt (0);
			if (startingViewController == null)
				return;

			DataViewController [] viewControllers = { startingViewController };
			pageViewController.SetViewControllers (viewControllers, Forward, false, null);


			pageViewController.DataSource = DataSource;
			pageViewController.DidMoveToParentViewController (this);
		}

		[Action ("forwardButtonPressed:")]
		void forwardButtonPressed (UIButton sender)
		{
			currentIndex += 1;
			currentIndex = currentIndex % dataSource.NumberOfFlags;

			var viewController = dataSource.GetViewControllerAt (currentIndex);
			pageViewController.SetViewControllers (new UIViewController [] { viewController }, Forward, true, null);
		}

		[Action ("backButtonPressed:")]
		void backButtonPressed (UIButton sender)
		{
			currentIndex -= 1;
			if (currentIndex < 0)
				currentIndex = dataSource.NumberOfFlags - 1;

			var viewController = dataSource.GetViewControllerAt (currentIndex);
			pageViewController?.SetViewControllers (new UIViewController [] { viewController }, Reverse, true, null);
		}
	}
}
