using Foundation;
using UIKit;

namespace Notifications
{
    

    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private MyViewController viewController;
        private UIWindow window;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            viewController = new MyViewController();
            window.RootViewController = viewController;

            window.MakeKeyAndVisible();

            // check for a notification
            if (options != null)
            {
                // check for a local notification
                if (options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                {
                    var localNotification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
                    if (localNotification != null)
                    {
						UIAlertController okayAlertController = UIAlertController.Create (localNotification.AlertAction, localNotification.AlertBody, UIAlertControllerStyle.Alert);
						okayAlertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
						viewController.PresentViewController (okayAlertController, true, null);
                        // reset our badge
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                    }
                }
            }

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes (
					UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
				);

				app.RegisterUserNotificationSettings (notificationSettings);
			} 

            return true;
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
			UIAlertController okayAlertController = UIAlertController.Create (notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
			okayAlertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
			viewController.PresentViewController (okayAlertController, true, null);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}
