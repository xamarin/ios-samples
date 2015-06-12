using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace Lifecycle.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		//Recall that AppDelegate subscribes to events from the system (subscribes to iOS events)

		UIWindow window;
		AppLifecycleViewController viewController;
		
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			window = new UIWindow(UIScreen.MainScreen.Bounds);
			
			viewController = new AppLifecycleViewController();
			window.RootViewController = viewController;
			
			window.MakeKeyAndVisible();
			
			return true;
		}

		public override void OnActivated(UIApplication application)
		{
			Console.WriteLine("OnActivated called, App is active.");
		}
		
		public override void WillEnterForeground(UIApplication application)
		{
			Console.WriteLine("App will enter foreground");
		}
		
		public override void OnResignActivation(UIApplication application)
		{
			Console.WriteLine("OnResignActivation called, App moving to inactive state.");
		}
		
		public override void DidEnterBackground(UIApplication application)
		{
			Console.WriteLine("App entering background state.");
		}
		
		// [not guaranteed that this will run]
		public override void WillTerminate(UIApplication application)
		{
			Console.WriteLine("App is terminating.");
		}
		
	}
}

