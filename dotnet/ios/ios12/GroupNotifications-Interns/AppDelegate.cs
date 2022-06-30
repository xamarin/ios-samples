using UserNotifications;

namespace GroupNotifications_Interns;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

    //public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
    //{
    //	// create a new window instance based on the screen size
    //	Window = new UIWindow (UIScreen.MainScreen.Bounds);

    //	// create a UIViewController with a single UILabel
    //	var vc = new UIViewController ();
    //	vc.View!.AddSubview (new UILabel (Window!.Frame) {
    //		BackgroundColor = UIColor.SystemBackground,
    //		TextAlignment = UITextAlignment.Center,
    //		Text = "Hello, iOS!",
    //		AutoresizingMask = UIViewAutoresizing.All,
    //	});
    //	Window.RootViewController = vc;

    //	// make the window visible
    //	Window.MakeKeyAndVisible ();

    //	return true;
    //}



    //Methods copied from GroupNotifications (old)
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
