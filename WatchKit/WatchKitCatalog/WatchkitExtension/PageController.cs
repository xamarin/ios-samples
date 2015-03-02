/*
 * This controller represents a single page of the modal page-based navigation controller, 
 * presented in AAPLControllerDetailController.
*/

using System;

using UIKit;
using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	public partial class PageController : WKInterfaceController
	{
		public PageController ()
		{
		}

		public override void Awake (NSObject context)
		{
			pageLabel.SetText (string.Format ("{0} Page", context));
		}

		public override void WillActivate ()
		{
			// This method is called when the controller is about to be visible to the wearer.
			Console.WriteLine (string.Format ("{0} will activate", this));
		}

		public override void DidDeactivate ()
		{
			// This method is called when the controller is no longer visible.
			Console.WriteLine (string.Format ("{0} did deactivate", this));
		}
	}
}

