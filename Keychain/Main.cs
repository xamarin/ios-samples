using System.Threading;
using Foundation;
using Security;
using UIKit;

namespace Keychain {
	public class Application {
		static void Main (string [] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate {
		UIViewController viewController;

		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			ThreadPool.QueueUserWorkItem (delegate
			{
				window.BeginInvokeOnMainThread (delegate
				{
					var rec = new SecRecord (SecKind.GenericPassword) {
						Generic = NSData.FromString ("foo")
					};

					SecStatusCode res;
					var match = SecKeyChain.QueryAsRecord (rec, out res);
					if (res == SecStatusCode.Success)
						DisplayMessage (this, "Key found, password is: {0}", match.ValueData);
					else
						DisplayMessage (this, "Key not found: {0}", res);

					var s = new SecRecord (SecKind.GenericPassword) {
						Label = "Item Label",
						Description = "Item description",
						Account = "Account",
						Service = "Service",
						Comment = "Your comment here",
						ValueData = NSData.FromString ("my-secret-password"),
						Generic = NSData.FromString ("foo")
					};

					var err = SecKeyChain.Add (s);

					if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
						DisplayMessage (this, "Error adding record: {0}", err);
				});
			});

			viewController = new UIViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			return true;
		}

		static void DisplayMessage (AppDelegate instance, string message, params object [] format)
		{
			new UIAlertView ("Keychain", string.Format (message, format), null, "OK", null).Show ();
		}
	}
}

