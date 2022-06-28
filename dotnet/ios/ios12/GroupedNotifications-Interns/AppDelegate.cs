using UserNotifications;

namespace GroupedNotifications_Interns;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate
{
	public override UIWindow? Window {
		get;
		set;
	}

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        UNUserNotificationCenter center = UNUserNotificationCenter.Current;
        center.RequestAuthorization(UNAuthorizationOptions.Alert, (bool success, NSError error) => {
            // Set the Delegate regardless of success; users can modify their notification
            // preferences at any time in the Settings app.
            center.Delegate = this;
        });
        return true;
    }

    [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
    public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, System.Action<UNNotificationPresentationOptions> completionHandler)
    {
        completionHandler(UNNotificationPresentationOptions.Alert);
    }
}
