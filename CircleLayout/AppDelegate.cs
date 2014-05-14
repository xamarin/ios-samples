using Foundation;
using UIKit;

namespace CircleLayout
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			window.RootViewController = new ViewController (new CircleLayout ());
			window.MakeKeyAndVisible();
			
			return true;
		}
		
		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}