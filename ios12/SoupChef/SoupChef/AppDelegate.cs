
namespace SoupChef
{
    using System;
    using System.Linq;
    using Foundation;
    using Intents;
    using SoupKit.Support;
    using UIKit;

    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            var intent = userActivity.GetInteraction()?.Intent as OrderSoupIntent;
            if (userActivity.ActivityType == typeof(OrderSoupIntent).Name ||
                userActivity.ActivityType == NSUserActivityHelper.ViewMenuActivityType ||
                userActivity.ActivityType == NSUserActivityHelper.OrderCompleteActivityType)
            {
                if (Window != null && 
                    Window.RootViewController is UINavigationController rootViewController)
                {
                    // The `restorationHandler` passes the user activity to the passed in view controllers to route the user to the part of the app
                    // that is able to continue the specific activity. See `restoreUserActivityState` in `OrderHistoryTableViewController`
                    // to follow the continuation of the activity further.
                    completionHandler(rootViewController.ViewControllers);

                    return true;
                }
                else 
                {
                    Console.WriteLine("Failed to access root view controller.");
                    return false;
                }
            }
            else 
            {
                Console.WriteLine($"Can't continue unknown NSUserActivity type {userActivity.ActivityType}");
                return false;
            }
        } 
    }
}