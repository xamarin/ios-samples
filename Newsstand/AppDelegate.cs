
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Newsstand
{
	/// <summary>
	/// Sizes the window according to the screen, for iPad as well as iPhone support
	/// </summary>
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var v = new NewsstandViewController();
			
			window = new UIWindow (UIScreen.MainScreen.Bounds);	
			window.BackgroundColor = UIColor.White;
			window.Bounds = UIScreen.MainScreen.Bounds;
			window.AddSubview(v.View);
            window.MakeKeyAndVisible ();
			return true;
		}
	}
}