/*
 * This controller displays switches and their various configurations.
*/

using System;

using UIKit;
using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	public partial class SwitchDetailController : WKInterfaceController
	{
		public SwitchDetailController ()
		{
			// offSwitch.SetOn (false);

			// coloredSwitch.SetColor (UIColor.Blue);
			// coloredSwitch.SetTitle ("Blue Switch");
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

		partial void SwitchAction (Boolean value)
		{
			Console.WriteLine ("Switch value changed to: {0}", value);
		}
	}
}

