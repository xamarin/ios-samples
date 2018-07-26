using Foundation;
using UIKit;

namespace PhotoAlbum
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUISplitViewControllerDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {

            // Override point for customization after application launch.
            var splitViewController = (UISplitViewController)Window.RootViewController;
            var navigationController = (UINavigationController)splitViewController.ViewControllers[1];
            navigationController.TopViewController.NavigationItem.LeftBarButtonItem = splitViewController.DisplayModeButtonItem;
            splitViewController.WeakDelegate = this;

            return true;
        }


        [Export("splitViewController:collapseSecondaryViewController:ontoPrimaryViewController:")]
        public bool CollapseSecondViewController(UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController)
        {
            //if (secondaryViewController.GetType() == typeof(UINavigationController) &&
            //    ((UINavigationController)secondaryViewController).TopViewController.GetType() == typeof(PhotoCollectionViewController) &&
            //    ((PhotoCollectionViewController)((UINavigationController)secondaryViewController).TopViewController).GetItemsCount(null, 0) > 0)
            //{
            //    // Return YES to indicate that we have handled the collapse by doing nothing; the secondary controller will be discarded.
            //    return true;
            //}
            return false;
        }
    }
}

