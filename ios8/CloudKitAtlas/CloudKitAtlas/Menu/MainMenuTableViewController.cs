using System;

using UIKit;
using Foundation;

namespace CloudKitAtlas
{
	public partial class MainMenuTableViewController : UITableViewController
	{
		public CodeSampleGroup [] CodeSampleGroups { get; set; }

		public MainMenuTableViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoadMenu ();
		}

		public override void ViewDidAppear (bool animated)
		{
			var lastIndex = CodeSampleGroups.Length - 1;
			if (lastIndex >= 0) {
				var sample = CodeSampleGroups [lastIndex].CodeSamples [0] as MarkNotificationsReadSample;
				if (sample != null)
					TableView.ReloadRows (new NSIndexPath [] { NSIndexPath.FromRowSection (lastIndex, 0) }, UITableViewRowAnimation.None);
			}
		}

		void LoadMenu ()
		{
			var discoverability = new CodeSampleGroup ("Discoverability", UIImage.FromBundle ("Discoverability"), new CodeSample [] {
				new RequestApplicationPermissionSample (),
				new FetchUserRecordIdSample (),
				new DiscoverUserInfoWithUserRecordIdSample (),
				new DiscoverUserInfoWithEmailAddressSample (),
				new DiscoverAllContactUserInfosSample ()
			});

			var zones = new CodeSampleGroup ("Zones", UIImage.FromBundle ("Zones"), new CodeSample [] {
				new SaveRecordZoneSample (),
				new DeleteRecordZoneSample (),
				new FetchRecordZoneSample (),
				new FetchAllRecordZonesSample ()
			});

			var query = new CodeSampleGroup ("Query", UIImage.FromBundle ("Query"), new CodeSample [] {
				new PerformQuerySample ()
			});

			var records = new CodeSampleGroup ("Records", UIImage.FromBundle ("Records"), new CodeSample [] {
				new SaveRecordSample (),
				new DeleteRecordSample (),
				new FetchRecordSample ()
			});

			var sync = new CodeSampleGroup ("Sync", UIImage.FromBundle ("Sync"), new CodeSample []{
				new FetchRecordChangesSample ()
			});

			var subscriptions = new CodeSampleGroup ("Subscriptions", UIImage.FromBundle ("Subscriptions"), new CodeSample [] {
				new SaveSubscriptionSample (),
				new DeleteSubscriptionSample (),
				new FetchSubscriptionSample (),
				new FetchAllSubscriptionsSample ()
			});

			var notifications = new CodeSampleGroup ("Notifications", UIImage.FromBundle ("Notifications"), new CodeSample [] {
				new MarkNotificationsReadSample ()
			});

			CodeSampleGroups = new CodeSampleGroup [] { discoverability, query, zones, records, sync, subscriptions, notifications };
		}

		#region Table view data source

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return CodeSampleGroups.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var codeSampleGroup = CodeSampleGroups [indexPath.Row];
			var cell = (MainMenuTableViewCell)tableView.DequeueReusableCell ("MainMenuItem", indexPath);
			cell.MenuLabel.Text = codeSampleGroup.Title;
			cell.MenuIcon.Image = codeSampleGroup.Icon;

			if (codeSampleGroup.CodeSamples.Length > 1) {
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			} else if (codeSampleGroup.CodeSamples.Length == 1) {
				var notificationSample = codeSampleGroup.CodeSamples [0] as MarkNotificationsReadSample;
				if (notificationSample != null && UIApplication.SharedApplication.IsRegisteredForRemoteNotifications) {
					if (notificationSample.Cache.AddedIndices.Count > 0) {
						cell.BadgeLabel.Superview.Layer.CornerRadius = cell.BadgeLabel.Font.PointSize * (nfloat)1.2 / 2;
						cell.BadgeLabel.Text = notificationSample.Cache.AddedIndices.Count.ToString ();
						cell.BadgeLabel.Superview.Hidden = false;
						cell.BadgeLabel.Hidden = false;
					} else {
						cell.BadgeLabel.Hidden = true;
						cell.BadgeLabel.Superview.Hidden = true;
					}
				}
			}
			return cell;
		}

		#endregion

		#region Navigation

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var codeSampleGroup = CodeSampleGroups [indexPath.Row];
			var count = codeSampleGroup.CodeSamples.Length;
			string segueIdentifier = count > 1 ? "ShowSubmenu" : "ShowCodeSampleFromMenu";

			PerformSegue (segueIdentifier, this);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			var indexPath = TableView.IndexPathForSelectedRow;
			if (indexPath != null) {
				var selectedCodeSampleGroup = CodeSampleGroups [indexPath.Row];
				if (segue.Identifier == "ShowSubmenu") {
					var submenuViewController = (SubmenuTableViewController)segue.DestinationViewController;
					submenuViewController.CodeSamples = selectedCodeSampleGroup.CodeSamples;
					submenuViewController.GroupTitle = selectedCodeSampleGroup.Title;
				} else if (segue.Identifier == "ShowCodeSampleFromMenu" && selectedCodeSampleGroup.CodeSamples.Length > 0) {
					var codeSampleViewController = (CodeSampleViewController)segue.DestinationViewController;
					codeSampleViewController.SelectedCodeSample = selectedCodeSampleGroup.CodeSamples [0];
					codeSampleViewController.GroupTitle = selectedCodeSampleGroup.Title;
				}
			}
		}

		#endregion
	}
}