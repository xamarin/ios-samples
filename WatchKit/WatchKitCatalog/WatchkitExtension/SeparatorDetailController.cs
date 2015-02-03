/*
 * This controller displays separators and how to visually break up content onscreen.
*/

using System;

using UIKit;
using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	[Register ("SeparatorDetailController")]
	public class SeparatorDetailController : WKInterfaceController
	{
		public SeparatorDetailController ()
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

