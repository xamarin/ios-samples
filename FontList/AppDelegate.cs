using System;
using UIKit;
using Foundation;

namespace FontList {
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		protected UIWindow window;
		protected UINavigationController mainNavController;
		protected FontList.Screens.Universal.FontListing.FontFamiliesTableViewController home;

		/// <summary>
		/// The current device (iPad or iPhone)
		/// </summary>
		public DeviceType CurrentDevice { get; set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create our window
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
			
			// are we running an iPhone or an iPad?
			DetermineCurrentDevice ();

			// instantiate our main navigatin controller and add it's view to the window
			mainNavController = new UINavigationController ();
			
			switch (CurrentDevice)
			{
				case DeviceType.iPhone:
				case DeviceType.iPad:
					home = new FontList.Screens.Universal.FontListing.FontFamiliesTableViewController ();
					mainNavController.PushViewController (home, false);
					break;
			}
			
			window.RootViewController = mainNavController;

			return true;
		}

		protected void DetermineCurrentDevice ()
		{
			// figure out the current device type
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				CurrentDevice = DeviceType.iPad;
			} else {
				CurrentDevice = DeviceType.iPhone;
			}
		}
	}
}