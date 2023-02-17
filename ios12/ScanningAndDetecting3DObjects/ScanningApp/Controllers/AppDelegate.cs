using ARKit;
using Foundation;
using UIKit;
using System;
using System.Xml.Serialization;

namespace ScanningAndDetecting3DObjects {
	[Register ("AppDelegate")]
	internal class AppDelegate : UIApplicationDelegate {
		internal static void FatalError (string msg)
		{
			Console.WriteLine (msg);
			throw new Exception (msg);
		}

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			//Note regarding the compiler warning: These are valid C# warnings, as these will resolve against ARConfiguration's static property. Needs library fix. 
			// 
			if (!(ARObjectScanningConfiguration.IsSupported && ARWorldTrackingConfiguration.IsSupported)) {
				FatalError (@"
				ARKit is not available on this device.For apps that require ARKit
				for core functionality, use the `arkit` key in the key in the
                `UIRequiredDeviceCapabilities` section of the Info.plist to prevent
				the app from installing. (If the app can't be installed, this error
				can't be triggered in a production scenario.)

				In apps where AR is an additive feature, use `isSupported` to
				determine whether to show UI for launching AR experiences.
			");
			}
			return true;
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, NSDictionary options)
		{
			var viewController = Window.RootViewController as ViewController;
			if (viewController != null) {
				viewController.ModelUrl = url;
				return true;
			} else {
				return false;
			}
		}

		public override void WillEnterForeground (UIApplication application)
		{
			var viewController = Window.RootViewController as ViewController;
			if (viewController != null) {
				viewController.BackFromBackground ();
			}
		}

		public override void OnResignActivation (UIApplication application)
		{
			var viewController = Window.RootViewController as ViewController;
			if (viewController != null) {
				viewController.ShowBlurView (false);
			}
		}

		public override void OnActivated (UIApplication application)
		{
			var viewController = Window.RootViewController as ViewController;
			if (viewController != null) {
				viewController.ShowBlurView (true);
			}
		}
	}
}

