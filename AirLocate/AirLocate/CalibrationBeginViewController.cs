using System;
using System.Collections.Generic;
using CoreGraphics;
using CoreLocation;
using Foundation;
using UIKit;

namespace AirLocate
{
	public partial class CalibrationBeginViewController : UITableViewController
	{
		List<CLBeacon>[] beacons;
		CLLocationManager locationManager;
		List<CLBeaconRegion> rangedRegions;
		bool inProgress;

		CalibrationCalculator calculator;
		CalibrationEndViewController endViewController;
		UIProgressView progressBar;

		public CalibrationBeginViewController (UITableViewStyle style) : base (style)
		{
			Unknowns = new List<CLBeacon> ();
			Immediates = new List<CLBeacon> ();
			Nears = new List<CLBeacon> ();
			Fars = new List<CLBeacon> ();
			beacons = new List<CLBeacon> [4] { Unknowns, Immediates, Nears, Fars };
			// This location manager will be used to display beacons available for calibration.
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
			Title = "Calibration";

			progressBar = new UIProgressView (UIProgressViewStyle.Default) {
				AutoresizingMask = UIViewAutoresizing.FlexibleMargins
			};

			// Populate the regions for the beacons we're interested in calibrating.
			rangedRegions = new List<CLBeaconRegion> ();

			foreach (NSUuid uuid in Defaults.SupportedProximityUuids) {
				var region = new CLBeaconRegion (uuid, uuid.AsString ());
				rangedRegions.Add (region);
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// Start ranging to show the beacons available for calibration.
			StartRangingAllRegions ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			// Cancel calibration (if it was started) and stop ranging when the view goes away.
			if (inProgress)
				calculator.CancelCalibration ();

			StopRangingAllRegions ();
			base.ViewWillDisappear (animated);
		}

		// empty section are not shown in TableView so we must exclude them
		int GetNonEmptySection (int section)
		{
			if (inProgress)
				section--;
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

		public override nint NumberOfSections (UITableView tableView)
		{
			//  return Unknown + Immediate + Near + Far if any beacon in each
			int sections = 0;
			foreach (var group in beacons) {
				if (group.Count > 0)
					sections++;
			}
			if (inProgress)
				sections++;
			return sections;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			if (inProgress && (section == 0))
				return 	1;
			return beacons [GetNonEmptySection ((int)section)].Count;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			// the first section has no title when the progress bar is shown
			if (((section == 0) && inProgress) || (NumberOfSections (tableView) == 0))
				return null;

			return ((CLProximity)GetNonEmptySection ((int)section)).ToString ();
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			int i = indexPath.Section;
			string identifier = inProgress && i == 0 ? "ProgressCell" : "BeaconCell";

			UITableViewCell cell = tableView.DequeueReusableCell (identifier);
			if (cell == null) {
				// Show the indicator that denotes calibration is in progress.
				if (identifier == "ProgressCell") {
					cell = new UITableViewCell (UITableViewCellStyle.Default, identifier) {
						SelectionStyle = UITableViewCellSelectionStyle.None
					};

					progressBar.Center = new CGPoint (cell.Center.X, 17.0f);
					cell.ContentView.AddSubview (progressBar);

					UILabel label = new UILabel (new CGRect (0.0f, 0.0f, 300.0f, 15.0f)) {
						AutoresizingMask = UIViewAutoresizing.FlexibleMargins,
						BackgroundColor = UIColor.Clear,
						Center = new CGPoint (cell.Center.X, 30.0f),
						Font = UIFont.SystemFontOfSize (11.0f),
						Text = "Wave device side-to-side 1m away from beacon",
						TextAlignment = UITextAlignment.Center,
						TextColor = UIColor.DarkGray
					};
					cell.ContentView.AddSubview (label);
				} else {
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, identifier);
				}
			}

			if (identifier == "ProgressCell")
				return cell;

			CLBeacon beacon = beacons [GetNonEmptySection (indexPath.Section)] [indexPath.Row];

			cell.TextLabel.Text = beacon.ProximityUuid.AsString ();
			cell.TextLabel.Font = UIFont.SystemFontOfSize (20.0f);
			cell.DetailTextLabel.Text = String.Format ("Major: {0}  Minor: {1}  Acc: {2:0.00}m",
				beacon.Major, beacon.Minor, beacon.Accuracy);

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (inProgress)
				return;

			CLBeacon beacon = beacons [GetNonEmptySection (indexPath.Section)] [indexPath.Row];
			CLBeaconRegion region = Helpers.CreateRegion (beacon.ProximityUuid, beacon.Major, beacon.Minor);
			if (region == null)
				return;

			// We can stop ranging to display beacons available for calibration.
			StopRangingAllRegions ();
			// And we'll start the calibration process.
			calculator = new CalibrationCalculator (region, CompletionHandler);
			calculator.PerformCalibration ((sender, e) => {
				progressBar.SetProgress (e.PercentComplete, true);
			});

			progressBar.Progress = 0.0f;
			inProgress = true;
			TableView.ReloadData ();
		}

		void CompletionHandler (object sender, CalibrationCompletedEventArgs e)
		{
			if (e.Error != null) {
				if (View.Window != null) {
					var message = e.Error.UserInfo [NSError.LocalizedDescriptionKey].ToString ();
					new UIAlertView ("Unable to calibrate device", message, null, "OK", null).Show ();
					StartRangingAllRegions ();
				}
			} else {
				endViewController = new CalibrationEndViewController (e.MeasurePower);
				NavigationController.PushViewController (endViewController, true);
			}

			inProgress = false;
			calculator = null;
			TableView.ReloadData ();
		}

		void StartRangingAllRegions ()
		{
			foreach (CLBeaconRegion region in rangedRegions)
				locationManager.StartRangingBeacons (region);
		}

		void StopRangingAllRegions ()
		{
			foreach (CLBeaconRegion region in rangedRegions)
				locationManager.StopRangingBeacons (region);
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
