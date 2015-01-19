using System;
using System.Threading;
using System.Threading.Tasks;

using UIKit;
using Foundation;

namespace BackgroundExecution
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		UINavigationController mainNavController;
		HomeScreen iPhoneHome;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();

			// instantiate our main navigatin controller and add it's view to the window
			mainNavController = new UINavigationController ();

			iPhoneHome = new HomeScreen ();
			mainNavController.PushViewController (iPhoneHome, false);

			window.RootViewController = mainNavController;

			// how to check if multi-tasking is supported
			if (UIDevice.CurrentDevice.IsMultitaskingSupported) {
				// code here to change your app's behavior
			}

			return true;
		}

		public override void WillEnterForeground (UIApplication application)
		{
			Console.WriteLine ("App will enter foreground");
		}

		// Runs when the activation transitions from running in the background to
		// being the foreground application.
		public override void OnActivated (UIApplication application)
		{
			Console.WriteLine ("App is becoming active");
		}

		public override void OnResignActivation (UIApplication application)
		{
			Console.WriteLine ("App moving to inactive state.");
		}

		public override void DidEnterBackground (UIApplication application)
		{
			Console.WriteLine ("App entering background state.");

			// if you're creating a VOIP application, this is how you set the keep alive
			//UIApplication.SharedApplication.SetKeepAliveTimout(600, () => { /* keep alive handler code*/ });

			// register a long running task, and then start it on a new thread so that this method can return
			var taskID = UIApplication.SharedApplication.BeginBackgroundTask (null);
			Task.Factory.StartNew (() => FinishLongRunningTask (taskID));
		}

		void FinishLongRunningTask (nint taskID)
		{
			Console.WriteLine ("Starting task {0}", taskID);
			Console.WriteLine ("Background time remaining: {0}", UIApplication.SharedApplication.BackgroundTimeRemaining);

			// sleep for 15 seconds to simulate a long running task
			Thread.Sleep (15000);

			Console.WriteLine ("Task {0} finished", taskID);
			Console.WriteLine ("Background time remaining: {0}", UIApplication.SharedApplication.BackgroundTimeRemaining);

			// call our end task
			UIApplication.SharedApplication.EndBackgroundTask (taskID);
		}

		// [not guaranteed that this will run]
		public override void WillTerminate (UIApplication application)
		{
			Console.WriteLine ("App is terminating.");
		}
	}
}
