using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

[Register]
public class AppController : NSObject {
	UIWindow window;

	[Export ("applicationDidFinishLaunching:")]
	public void FinishedLaunching (UIApplication app)
	{
		SetupWindow ();
	}

	void SetupWindow ()
	{
		var window = new UIWindow (UIScreen.MainScreen.Bounds) {
			new UILabel (new RectangleF (50, 50, 230, 100)) {
				Text = "Hello from MonoTouch"
			}
		};
		
		window.MakeKeyAndVisible ();
	}


}

class Demo {
	static void Main (string [] args)
	{
		Console.WriteLine ("Launching");
		UIApplication.Main (args, null, "AppController");
		Console.WriteLine ("Returning from Main, this sucks");
	}		
}
