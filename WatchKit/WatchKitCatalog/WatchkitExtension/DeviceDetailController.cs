/* 
 * This controller displays device specific information to use for ensuring a great experience to the wearer of the WatchKit app.
*/

using System;

using WatchKit;
using CoreGraphics;

namespace WatchkitExtension
{
	public partial class DeviceDetailController : WKInterfaceController
	{
		public DeviceDetailController ()
		{
			CGRect bounds = WKInterfaceDevice.CurrentDevice.ScreenBounds;
			nfloat scale = WKInterfaceDevice.CurrentDevice.ScreenScale;

			boundsLabel.SetText (bounds.ToString ());
			scaleLabel.SetText (scale.ToString ());
			preferredContentSizeLabel.SetText (WKInterfaceDevice.CurrentDevice.PreferredContentSizeCategory.ToString ());
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
	}
}