using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using MonoTouch.Dialog;

namespace KannadaKeyboardCompanion
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;

		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			var web = new WebElement ();
			web.HtmlFile = "instructions";

			var root = new RootElement ("Kannada Keyboard") {
				new Section{
					new UIViewElement("Instruction", web.View, false)
				}
			};
		
			var dv = new DialogViewController (root) {
				Autorotate = true
			};
			var navigation = new UINavigationController ();
			navigation.PushViewController (dv, true);				

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.MakeKeyAndVisible ();
			window.AddSubview (navigation.View);
			
			return true;
		}
	}
}

