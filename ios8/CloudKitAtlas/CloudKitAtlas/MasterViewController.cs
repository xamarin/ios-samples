using System;
using Foundation;
using UIKit;
using CloudKit;

namespace CloudKitAtlas
{
	public partial class MasterViewController : UITableViewController
	{
		CloudManager cloudManager;

		public MasterViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			cloudManager = new CloudManager ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var destination = segue.DestinationViewController as ICloudViewController;
			destination.CloudManager = cloudManager;
		}
	}
}
