//
// Wayup sample in C#
//

using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

[Register]
public class CrateViewController : UIViewController {

	public CrateViewController (IntPtr handle) : base (handle) {}
	
	public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
	{
		return true;
	}
}

[Register]
public partial class WhichWayIsUpAppDelegate : UIApplicationDelegate {
	public override void FinishedLaunching (UIApplication app)
	{
		window.AddSubview (crateViewController.View);
	}
	
}

class Demo {
	static void Main (string [] args)
	{
		UIApplication.Main (args, null, null);
	}
}