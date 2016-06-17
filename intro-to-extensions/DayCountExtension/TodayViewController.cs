using System;
using System.Drawing;

using NotificationCenter;
using Foundation;
using Social;
using UIKit;
using CoreGraphics;

namespace DayCountExtension
{
	public partial class TodayViewController : SLComposeServiceViewController, INCWidgetProviding
	{
		public TodayViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var dayOfYear = DateTime.Now.DayOfYear;
			var leapYearExtra = DateTime.IsLeapYear (DateTime.Now.Year) ? 1 : 0;
			var daysRemaining = 365 + leapYearExtra - dayOfYear;

			var msg = String.Format ("Today is day {0}. There are {1} days remaining in the year.", dayOfYear, daysRemaining);
			this.extensionLabel.Text = msg;
		}

		public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
		{
			// Perform any setup necessary in order to update the view.

			// If an error is encoutered, use NCUpdateResultFailed
			// If there's no update required, use NCUpdateResultNoData
			// If there's an update, use NCUpdateResultNewData

			completionHandler (NCUpdateResult.NewData);
		}

	}
}

