using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace HelloGoodbye
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		private UIWindow _window;
		private StartViewController _startViewController;
		private UINavigationController _navigationController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			_window = new UIWindow (UIScreen.MainScreen.Bounds);

			_startViewController = new StartViewController ();
			_navigationController = new UINavigationController (_startViewController);
			_navigationController.NavigationBar.TintColor = StyleUtilities.ForegroundColor;

			_window.RootViewController = _navigationController;

			_window.MakeKeyAndVisible ();

			return true;
		}
	}
}

