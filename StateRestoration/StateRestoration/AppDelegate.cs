using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace StateRestoration
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		DataSource dataSource;

		public override UIWindow Window { get; set; }

		public override bool WillFinishLaunching (UIApplication application, NSDictionary launchOptions)
		{
			Window.TintColor = UIColor.LightGray;

			dataSource = new DataSource ();

			UIApplication.RegisterObjectForStateRestoration (dataSource, "DataSource");

			var navigationController = (UINavigationController)Window.RootViewController;
			navigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			navigationController.NavigationBar.Translucent = true;

			var viewController = (CollectionViewController)navigationController.ViewControllers [0];
			viewController.DataSource = dataSource;

			Window.MakeKeyAndVisible ();

			return true;
		}

		public override bool ShouldSaveApplicationState (UIApplication application, NSCoder coder)
		{
			return true;
		}

		public override bool ShouldRestoreApplicationState (UIApplication application, NSCoder coder)
		{
			return true;
		}
	}
}

