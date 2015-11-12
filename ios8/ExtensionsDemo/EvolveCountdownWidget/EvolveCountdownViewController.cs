using System;

using CoreGraphics;
using Foundation;
using NotificationCenter;
using UIKit;

namespace EvolveCountdownWidget {
	public partial class EvolveCountdownViewController : UIViewController, INCWidgetProviding {
		public EvolveCountdownViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			PreferredContentSize = new CGSize (PreferredContentSize.Width, 55f);

			UpdateTitle ();

			WidgetButton.SetTitle ("Tap here to register", UIControlState.Normal);
			WidgetButton.TouchUpInside += (sender, e) => UIApplication.SharedApplication.OpenUrl (new NSUrl ("evolveCountdown://"));
		}

		[Export ("widgetPerformUpdateWithCompletionHandler:")]
		public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
		{
			UpdateTitle ();
			completionHandler (NCUpdateResult.NewData);
		}

		void UpdateTitle()
		{
			var evolveStartDate = new DateTime (2014, 10, 6);
			var numDays = (evolveStartDate - DateTime.Now).Days;

			WidgetTitle.Text = String.Format ("{0} days until Evolve", numDays);
		}
	}
}
