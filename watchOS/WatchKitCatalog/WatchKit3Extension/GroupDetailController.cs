/*
 * This controller displays groups in various configurations. 
 * This controller demonstrates sophisticated layouts using nested groups.
*/

using System;

using UIKit;
using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("GroupDetailController")]
	public class GroupDetailController : WKInterfaceController
	{
		public GroupDetailController ()
		{
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

