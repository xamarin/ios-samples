
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace GoogleAnalytics
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	public partial class AppDelegate : UIApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// Replace with your account information.
			var account = "UA-000000-1";
			int time = 10;
			
			var tracker = GoogleAnalytics.GANTracker.SharedTracker;
			tracker.StartTracker (account, time, null);
			
			var label = new UILabel (new RectangleF (10, 10, 300, 20));
			                         
			NSError error;
			if (tracker.TrackPageView ("/app_entry_point", out error))
				label.Text = "Tracking successfully sent!";
			else
				label.Text = "Error starting tracker";
			
			// Force the event to be sent
			tracker.Dispatch ();
			
			window.AddSubview (label);
			                   
			window.MakeKeyAndVisible ();
			
			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}
