using System;
using System.Drawing;

using NotificationCenter;
using Foundation;
using Social;
using UIKit;
using CoreGraphics;

namespace EvolveCountdownWidget
{
	public partial class EvolveCountdownViewController : UIViewController, INCWidgetProviding
	{
		public EvolveCountdownViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			PreferredContentSize = new CGSize (PreferredContentSize.Width, 55f);
            var evolveStartDate = new DateTime (2014, 10, 6);
            var numDays = (evolveStartDate - DateTime.Now).Days;

            WidgetTitle.Text = String.Format ("{0} days until Evolve", numDays);

            WidgetButton.SetTitle ("Tap here to register", UIControlState.Normal);
                
            WidgetButton.TouchUpInside += (sender, e) => UIApplication.SharedApplication.OpenUrl (new NSUrl ("evolveCountdown://"));
		}

        //BUG: this never gets called
		public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
		{
			WidgetTitle.Text = "updated title";
	
			completionHandler (NCUpdateResult.NewData);
		}
	}
}