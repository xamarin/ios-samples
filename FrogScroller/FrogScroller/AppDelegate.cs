using System;

using Foundation;
using UIKit;

namespace FrogScroller
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		UIPageViewController viewController;

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			viewController = new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll, 
			                                          UIPageViewControllerNavigationOrientation.Horizontal,
			                                          UIPageViewControllerSpineLocation.None, 20f);

			// kick things off by making the first page
			ImageViewController pageZero = ImageViewController.ImageViewControllerForPageIndex (0);
			viewController.SetViewControllers (new UIViewController[] { pageZero },
			                                       UIPageViewControllerNavigationDirection.Forward,
			                                       false, null);
			viewController.DataSource = new MyDataSource ();
			window.RootViewController = viewController;
			// make the window visible
			window.MakeKeyAndVisible ();

			return true;
		}

		//DataSource PageIndexing
		class MyDataSource : UIPageViewControllerDataSource {
			
			public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, 
			                                                            UIViewController referenceViewController)
			{
				int index = ((ImageViewController) referenceViewController).PageIndex;
				return (ImageViewController)ImageViewController.ImageViewControllerForPageIndex(index - 1);
			}
			
			public override UIViewController GetNextViewController (UIPageViewController pageViewController, 
			                                                        UIViewController referenceViewController)
			{
				int index = ((ImageViewController) referenceViewController).PageIndex;
				return (ImageViewController)ImageViewController.ImageViewControllerForPageIndex (index + 1);
			}
		}
	}
}