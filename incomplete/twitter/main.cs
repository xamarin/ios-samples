using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

[Register]
public partial class TwitterSharpAppDelegate : UIApplicationDelegate {

	public TwitterSharpAppDelegate (IntPtr p) : base (p) { }

	public override void FinishedLaunching (UIApplication app)
	{
		Window.AddSubview (TabBarController.View);
	}
	       
	static void Main (string [] args)
	{
		UIApplication.Main (args, null, null);
	}
}