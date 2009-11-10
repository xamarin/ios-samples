/*
 * Sample metronome application, based on the Apple sample
 */
using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

[Register]
public class AppController : UIApplicationDelegate {
	UIWindow window;

	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
		Console.WriteLine ("Setting up");
		SetupWindow ();

		return true;
	}

	public override void OnActivated (UIApplication app)
	{
	}

	void SetupWindow ()
	{
		window = new UIWindow (UIScreen.MainScreen.Bounds);
		UILabel l = new UILabel (new RectangleF (50, 50, 230, 100)) {
			Text = "Hello from MonoTouch"
		};
		
		window.AddSubview (l);
		window.MakeKeyAndVisible ();
	}


}

class Demo {
	static void Main (string [] args)
	{
		Console.WriteLine ("On startup");
		UIApplication.Main (args, null, "AppController");
		Console.WriteLine ("Finish");
	}		
}
