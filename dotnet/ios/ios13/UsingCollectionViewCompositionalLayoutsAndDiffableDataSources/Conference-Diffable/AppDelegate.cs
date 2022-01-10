namespace Conference_Diffable;

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


	public static UIColor CornflowerBlue {
		get => UIColor.FromDisplayP3 (100.0f / 255.0f, 149.0f / 255.0f, 237.0f / 255.0f, 1.0f);
	}
}
