/*
 * This controller displays labels and specialized labels (Date and Timer).
*/

using System;

using UIKit;
using WatchKit;
using Foundation;

namespace WatchkitExtension
{
	public partial class LabelDetailController : WKInterfaceController
	{
		public LabelDetailController()
		{
			coloredLabel.SetTextColor(UIColor.Purple);

			var attr = new UIStringAttributes
			{
				Font = UIFont.SystemFontOfSize(18f, UIFontWeight.UltraLight)
			};
			var attrString = new NSAttributedString("Ultralight Label", attr.Dictionary);

			ultralightLabel.SetText(attrString);

			var components = new NSDateComponents {
			Day = 7,
			Month = 9,
			Year = 2016
			};
			timer.SetDate (NSCalendar.CurrentCalendar.DateFromComponents (components));
			timer.Start ();
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

