using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace WayUp
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	public partial class WhichWayIsUpAppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.RootViewController = crateViewController;
			window.MakeKeyAndVisible ();
	
			return true;
		}
	}
	
	public partial class CrateViewController : UIViewController {

		public CrateViewController (IntPtr handle) : base (handle) {}

		public override bool ShouldAutorotate ()
		{
			return true;
		}
	}
}

