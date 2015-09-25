using Foundation;
using UIKit;

namespace ViewControllerPreview {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate, IUISplitViewControllerDelegate {
		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			var splitViewController = Window?.RootViewController as UISplitViewController;
			splitViewController.Delegate = this;
			return true;
		}

		[Export ("splitViewController:collapseSecondaryViewController:ontoPrimaryViewController:")]
		public virtual bool CollapseSecondViewController (UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController)
		{
			var secondaryAsNavController = secondaryViewController as UINavigationController;

			if (secondaryAsNavController == null)
				return false;

			var topAsDetailController = secondaryAsNavController.TopViewController as DetailViewController;
			return topAsDetailController != null && string.IsNullOrEmpty (topAsDetailController.DetailItemTitle);
		}
	}
}

 
