using System;
using System.Collections.Generic;

using CoreLocation;
using Foundation;
using UIKit;

namespace AirLocate
{
	public partial class RangingViewController : UITableViewController
	{
		List<CLBeacon>[] beacons;
		CLLocationManager locationManager;
		List<CLBeaconRegion> rangedRegions;

		public RangingViewController (UITableViewStyle style) : base (style)
		{
			Unknowns = new List<CLBeacon> ();
			Immediates = new List<CLBeacon> ();
			Nears = new List<CLBeacon> ();
			Fars = new List<CLBeacon> ();
			beacons = new List<CLBeacon> [4] { Unknowns, Immediates, Nears, Fars };

			locationManager = new CLLocationManager ();
			locationManager.DidRangeBeacons += HandleDidRangeBeacons;
		}

		List<CLBeacon> Unknowns { get; set; }

		List<CLBeacon> Immediates { get; set; }

		List<CLBeacon> Nears { get; set; }

		List<CLBeacon> Fars { get; set; }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = "Ranging";

			// Populate the regions we will range once.
			rangedRegions = new List<CLBeaconRegion> ();

			foreach (NSUuid uuid in Defaults.SupportedProximityUuids) {
				CLBeaconRegion region = new CLBeaconRegion (uuid, uuid.AsString ());
				rangedRegions.Add (region);
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// Start ranging when the view appears.
			foreach (CLBeaconRegion region in rangedRegions)
				locationManager.StartRangingBeacons (region);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			foreach (CLBeaconRegion region in rangedRegions)
				locationManager.StopRangingBeacons (region);
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// skip empty groups
			int sections = 0;
			foreach (var group in beacons) {
				if (group.Count > 0)
					sections++;
			}
			return sections;
		}

		// empty section are not shown in TableView so we must exclude them
		int GetNonEmptySection (int section)
		{
			int current = 0;
			foreach (var group in beacons) {
				if (group.Count > 0) {
					if (section-- == 0)
						return current;
				}
				current++;
			}
			return -1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return beacons [GetNonEmptySection ((int)section)].Count;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			if (NumberOfSections (tableView) == 0)
				return null;

			return ((CLProximity)GetNonEmptySection ((int)section)).ToString ();
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell ("Cell");
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, "Cell");
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			}

			// Display the UUID, major, minor and accuracy for each beacon.
			CLBeacon beacon = beacons [GetNonEmptySection (indexPath.Section)] [indexPath.Row];

			cell.TextLabel.Text = beacon.ProximityUuid.AsString ();
			cell.DetailTextLabel.Text = String.Format ("Major: {0}  Minor: {1}  Acc: {2:0.00}m",
				beacon.Major, beacon.Minor, beacon.Accuracy);
			return cell;
		}

		void HandleDidRangeBeacons (object sender, CLRegionBeaconsRangedEventArgs e)
		{
			Unknowns.Clear ();
			Immediates.Clear ();
			Nears.Clear ();
			Fars.Clear ();

			foreach (CLBeacon beacon in e.Beacons) {
				switch (beacon.Proximity) {
				case CLProximity.Immediate:
					Immediates.Add (beacon);
					break;
				case CLProximity.Near:
					Nears.Add (beacon);
					break;
				case CLProximity.Far:
					Fars.Add (beacon);
					break;
				case CLProximity.Unknown:
					Unknowns.Add (beacon);
					break;
				}
			}

			TableView.ReloadData ();
		}
	}
}
