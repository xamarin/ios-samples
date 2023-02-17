using System.Linq;
using Foundation;
using UIKit;

namespace SamplePhotoApp {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate, IUISplitViewControllerDelegate {
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			var splitViewController = Window.RootViewController as UISplitViewController;
#if __IOS__
			var navigationController = (UINavigationController)splitViewController.ViewControllers.Last ();
			navigationController.TopViewController.NavigationItem.LeftBarButtonItem = splitViewController.DisplayModeButtonItem;
#endif
			splitViewController.Delegate = this;

			return true;
		}

		[Export ("splitViewController:collapseSecondaryViewController:ontoPrimaryViewController:")]
		public bool CollapseSecondViewController (UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController)
		{
			var secondaryAsNavController = secondaryViewController as UINavigationController;
			if (secondaryAsNavController == null)
				return false;

			var topAsDetailController = secondaryAsNavController.TopViewController as AssetGridViewController;
			if (topAsDetailController == null)
				return false;

			// Return true to indicate that we have handled the collapse by doing nothing; the secondary controller will be discarded.
			return topAsDetailController.FetchResult == null;
		}

		[Export ("splitViewController:showDetailViewController:sender:")]
		public bool EventShowDetailViewController (UISplitViewController splitViewController, UIViewController vc, NSObject sender)
		{
			// Let the storyboard handle the segue for every case except going from detail:assetgrid to detail:asset.
			if (splitViewController.Collapsed)
				return false;

			if (vc is UINavigationController)
				return false;

			var detailNavController = splitViewController.ViewControllers.Last () as UINavigationController;
			if (detailNavController == null || detailNavController.ViewControllers.Length == 1)
				return false;

			detailNavController.PushViewController (vc, true);

			return true;
		}
	}
}
