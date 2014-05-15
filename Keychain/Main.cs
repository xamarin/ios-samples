//
// Shows how to use the MonoTouch.Security stack on iOS5 to
// securely store a password on the KeyChain.
//
// This API is not particularly user-friendly
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Foundation;
using UIKit;
using Security;

namespace Keychain
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIViewController viewController;
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			ThreadPool.QueueUserWorkItem (delegate {
				window.BeginInvokeOnMainThread (delegate {
					var rec = new SecRecord (SecKind.GenericPassword){
						Generic = NSData.FromString ("foo")
					};
			
					SecStatusCode res;
					var match = SecKeyChain.QueryAsRecord (rec, out res);
					if (res == SecStatusCode.Success)
						DisplayMessage ("Key found, password is: {0}", match.ValueData);
					else
						DisplayMessage ("Key not found: {0}", res);
					
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
						DisplayMessage ("Error adding record: {0}", err);
				});
			});
						
			viewController = new UIViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			
			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
		
		void DisplayMessage (string message, params object[] format)
		{
			new UIAlertView ("Keychain", string.Format (message, format), null, "OK", null).Show ();
		}
	}
}

