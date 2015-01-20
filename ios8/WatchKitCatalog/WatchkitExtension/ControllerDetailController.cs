/*
 * This controller demonstrates how to present a modal controller with a page-based navigation style. 
 * By performing a Force Touch gesture on the controller (click-and-hold in the iOS Simulator), you can present a menu.
*/

using System;

using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	public partial class ControllerDetailController : WKInterfaceController
	{
		public ControllerDetailController ()
		{
		}

		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine ("{0} did deactivate", this);
		}

		partial void PresentPages (NSObject obj)
		{
			var controllerNames = new [] { "pageController", "pageController", "pageController", "pageController", "pageController" };
			var contexts = new [] { "First", "Second", "Third", "Fourth", "Fifth" };
			PresentController (controllerNames, contexts);
		}

		partial void MenuItemTapped (NSObject obj)
		{
			Console.WriteLine ("A menu item was tapped.");
		}
	}
}