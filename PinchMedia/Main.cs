
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace PinchMedia
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
			string applicationCode = "REPLACE WITH YOUR OWN APPLICATION CODE";
    			Beacon.StartBeacon(applicationCode, false, false, true); 
			
			var label = new UILabel (new RectangleF (10, 30, 300, 20));
			label.Text = "Tracking with Pinch Media Analytics";                         
			
			window.AddSubview (label);
			                   
			window.MakeKeyAndVisible ();
			
			return true;
		}

		public override void WillTerminate (UIApplication application)
		{
			// (your app's cleanup code)
    
    			// CALL THIS LAST! (See this FAQ entry for more info)
    			Beacon.EndBeacon();
		}

		
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}
