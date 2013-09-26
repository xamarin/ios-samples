using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CalabashSample.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		MainViewController mainViewController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			mainViewController = new MainViewController ();
			window.RootViewController = mainViewController;
			window.MakeKeyAndVisible ();
			
			#if DEBUG
			// This is all you need to do.
			// If you are in release mode, this
			// call isn't made, and the Xamarin
			// linker will remove Calabash
			// from your app.
			Xamarin.Calabash.Start();
			#endif

			return true;
		}
	}
}

