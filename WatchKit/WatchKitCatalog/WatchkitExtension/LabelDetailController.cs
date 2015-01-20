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
		public LabelDetailController ()
		{
			// Custom fonts must be added to the Watch app target and referenced in the Watch app's Info.plist.
			var attributedString = new NSAttributedString ("Custom Font", new UIStringAttributes {
				Font = UIFont.FromName("Menlo", 12f)
			});

			customFontLabel.SetAttributedText (attributedString);
			coloredLabel.SetTextColor (UIColor.Purple);

			var components = new NSDateComponents {
				Day = 10,
				Month = 12,
				Year = 2015
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

