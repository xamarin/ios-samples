using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CocosSharp;

namespace BouncingGame.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override void FinishedLaunching (UIApplication app)
		{
			var application = new CCApplication ();
			application.ApplicationDelegate = new BouncingGame.GameAppDelegate ();
			application.StartGame ();
		}
	}
}

