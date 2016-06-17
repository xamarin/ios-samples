using System;

using UIKit;
using Foundation;

namespace Reachability {
	public partial class MainViewController : UITableViewController {
		const string ReachabilityTableCellIdentifier = "ReachabilityTableCell";

		UIImage imageCarrier, imageWiFi, imageStop;
		NetworkStatus remoteHostStatus, internetStatus, localWifiStatus;

		public MainViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			UpdateStatus (null, null);
			Reachability.ReachabilityChanged += UpdateStatus;

			imageCarrier = UIImage.FromFile ("WWAN5.png");
			imageWiFi = UIImage.FromFile ("Airport.png");
			imageStop = UIImage.FromFile ("stop-32.png");

			TableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			TableView.RowHeight = 44.0f;
			TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			TableView.SectionHeaderHeight = 28.0f;
			TableView.ScrollEnabled = false;
		}

		public override NSIndexPath WillSelectRow (UITableView view, NSIndexPath index)
		{
			return null;
		}

		public override nint RowsInSection (UITableView view, nint section)
		{
			return 1;
		}

		public override nint NumberOfSections (UITableView view)
		{
			return 3;
		}

		public override string TitleForHeader (UITableView view, nint section)
		{
			switch (section) {
			case 0:
				return Reachability.HostName;
			case 1:
				return "Access to internet hosts";
			case 2:
				return "Access to Local Bonjour Hosts";
			default:
				return "Unknown";
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (ReachabilityTableCellIdentifier);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, ReachabilityTableCellIdentifier);
				var label = cell.TextLabel;
				label.Font = UIFont.SystemFontOfSize (12f);
				label.TextColor = UIColor.DarkGray;
				label.TextAlignment = UITextAlignment.Left;
			}

			string text = "";
			UIImage image = null;
			switch (indexPath.Section) {
			case 0:
				switch (remoteHostStatus) {
				case NetworkStatus.NotReachable:
					text = "Cannot connect to remote host";
					image = imageStop;
					break;
				case NetworkStatus.ReachableViaCarrierDataNetwork:
					text = "Reachable via data carrier network";
					image = imageCarrier;
					break;
				case NetworkStatus.ReachableViaWiFiNetwork:
					text = "Reachable via WiFi network";
					image = imageWiFi;
					break;
				}
				break;
			case 1:
				switch (internetStatus) {
				case NetworkStatus.NotReachable:
					text = "Access not available";
					image = imageStop;
					break;
				case NetworkStatus.ReachableViaCarrierDataNetwork:
					text = "Available via data carrier network";
					image = imageCarrier;
					break;
				case NetworkStatus.ReachableViaWiFiNetwork:
					text = "Available via WiFi network";
					image = imageWiFi;
					break;
				}
				break;
			case 2:
				switch (localWifiStatus) {
				case NetworkStatus.NotReachable:
					text = "Access not available";
					image = imageStop;
					break;
				case NetworkStatus.ReachableViaWiFiNetwork:
					text = "Available via WiFi network";
					image = imageWiFi;
					break;
				}
				break;
			}
			cell.TextLabel.Text = text;
			cell.ImageView.Image = image;
			return cell;
		}

		void UpdateStatus (object sender, EventArgs e)
		{
			remoteHostStatus = Reachability.RemoteHostStatus ();
			internetStatus = Reachability.InternetConnectionStatus ();
			localWifiStatus = Reachability.LocalWifiConnectionStatus ();
			TableView.ReloadData ();
		}
	}
}
