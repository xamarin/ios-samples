using System;
using Foundation;
using UIKit;

namespace CloudKitAtlas
{
	public partial class CKSubscriptionViewController : UITableViewController, ICloudViewController
	{
		public CloudManager CloudManager { get; set; }

		public CKSubscriptionViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			subscriptionSwitch.On = CloudManager.Subscribed;
		}

		partial void SubscriptionPreferenceUpdated (UISwitch sender)
		{
			if (sender.On) {
				CloudManager.Subscribe ();
			} else {
				CloudManager.Unsubscribe ();
			}
		}
	}
}
