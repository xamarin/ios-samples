using Foundation;
using UIKit;

namespace GLKBaseEffectDrawingTexture
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		MCViewController controller;

		public override UIWindow Window { get; set; }

		public override void FinishedLaunching (UIApplication application)
		{
			Window = new UIWindow (UIScreen.MainScreen.Bounds);

			controller = new MCViewController ();
			Window.RootViewController = controller;

			// make the window visible
			Window.MakeKeyAndVisible ();
		}
	}
}

