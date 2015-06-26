using Foundation;
using UIKit;

namespace Cloud {
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate {
		UIWindow window;
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var v = new KeyValueViewController();

			window = new UIWindow (UIScreen.MainScreen.Bounds);	
			window.BackgroundColor = UIColor.White;
			window.Bounds = UIScreen.MainScreen.Bounds;
			window.AddSubview(v.View);
            window.MakeKeyAndVisible ();
			return true;
		}
	}
}