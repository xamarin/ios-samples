using UIKit;
using Foundation;

namespace OpenGLESSample
{
	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class OpenGLESSampleAppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			var rootViewController = new UIViewController {
				View = glView
			};
			window.RootViewController = rootViewController;
			glView.AnimationInterval = 1.0 / 60.0;
			glView.StartAnimation();

			window.MakeKeyAndVisible ();

			return true;
		}

		public override void OnResignActivation (UIApplication application)
		{
			glView.AnimationInterval = 1.0 / 5.0;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
			glView.AnimationInterval = 1.0 / 60.0;
		}
	}
}

