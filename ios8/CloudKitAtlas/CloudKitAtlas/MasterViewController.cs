using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CloudKit;

namespace CloudKitAtlas
{
	public partial class MasterViewController : UITableViewController
	{
		private CloudManager cloudManager;

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
