namespace Gallery;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// Override point for customization after application launch.
		// If not required for your application you can safely delete this method

		return true;
	}

	public override UISceneConfiguration GetConfiguration (UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
			=> new UISceneConfiguration ("Default Configuration", connectingSceneSession.Role);
}
