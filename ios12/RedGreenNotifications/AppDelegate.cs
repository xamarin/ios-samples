using Foundation;
using UIKit;
using UserNotifications;
using System;

namespace RedGreenNotifications
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        public const string InitializedNotificationSettingsKey = "InitializedNotificationSettings";

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Request authorization to send notifications
            UNUserNotificationCenter center = UNUserNotificationCenter.Current;
            var options = UNAuthorizationOptions.ProvidesAppNotificationSettings | UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Provisional;
            center.RequestAuthorization(options, (bool success, NSError error) =>
            {
                center.Delegate = this;

                var rotateTwentyDegreesAction = UNNotificationAction.FromIdentifier("rotate-twenty-degrees-action", "Rotate 20°", UNNotificationActionOptions.None);

                var redCategory = UNNotificationCategory.FromIdentifier(
                    "red-category",
                    new UNNotificationAction[] { rotateTwentyDegreesAction },
                    new string[] { },
                    UNNotificationCategoryOptions.CustomDismissAction
                );

                var greenCategory = UNNotificationCategory.FromIdentifier(
                    "green-category",
                    new UNNotificationAction[] { rotateTwentyDegreesAction },
                    new string[] { },
                    UNNotificationCategoryOptions.CustomDismissAction
                );

                var set = new NSSet<UNNotificationCategory>(redCategory, greenCategory);
                center.SetNotificationCategories(set);
            });

            // Initialize granular notification settings on first run
            bool initializedNotificationSettings = NSUserDefaults.StandardUserDefaults.BoolForKey(InitializedNotificationSettingsKey);
            if (!initializedNotificationSettings)
            {
                NSUserDefaults.StandardUserDefaults.SetBool(true, ManageNotificationsViewController.RedNotificationsEnabledKey);
                NSUserDefaults.StandardUserDefaults.SetBool(true, ManageNotificationsViewController.GreenNotificationsEnabledKey);
                NSUserDefaults.StandardUserDefaults.SetBool(true, InitializedNotificationSettingsKey);
            }
            return true;
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, System.Action completionHandler)
        {
            if (response.IsDefaultAction)
            {
                Console.WriteLine("ACTION: Default");
            }
            if (response.IsDismissAction)
            {
                Console.WriteLine("ACTION: Dismiss");
            }
            else
            {
                Console.WriteLine($"ACTION: {response.ActionIdentifier}");
            }

            completionHandler();
        }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, System.Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
        }

        [Export("userNotificationCenter:openSettingsForNotification:")]
        public void OpenSettings(UNUserNotificationCenter center, UNNotification notification)
        {
            var navigationController = Window.RootViewController as UINavigationController;
            if (navigationController != null)
            {
                var currentViewController = navigationController.VisibleViewController;
                if (currentViewController is ViewController)
                {
                    currentViewController.PerformSegue(ManageNotificationsViewController.ShowManageNotificationsSegue, this);
                }
            }
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {

        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}

