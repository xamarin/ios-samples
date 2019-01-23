using Foundation;
using UIKit;

namespace TableSearch
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            return true;
        }

        public override bool ShouldSaveApplicationState(UIApplication application, NSCoder coder)
        {
            return true;
        }

        public override bool ShouldRestoreApplicationState(UIApplication application, NSCoder coder)
        {
            return true;
        }
    }
}