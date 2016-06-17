using System;
using System.Linq;

using Contacts;
using CoreLocation;
using Foundation;
using UIKit;

namespace HomeKitCatalog
{
	// A view controller which facilitates the creation of a location trigger.
	public partial class LocationTriggerViewController : EventTriggerViewController
	{
		static readonly NSString LocationCell = (NSString)"LocationCell";
		static readonly NSString RegionStatusCell = (NSString)"RegionStatusCell";
		const string selectLocationSegue = "Select Location";

		static readonly CLGeocoder geocoder = new CLGeocoder ();

		static readonly string[] RegionStatusTitles = {
			"When I Enter The Area",
			"When I Leave The Area"
		};

		string localizedAddress;
		bool viewIsDisplayed;


		LocationTriggerCreator LocationTriggerCreator {
			get {
				return (LocationTriggerCreator)TriggerCreator;
			}
		}

		#region ctors

		public LocationTriggerViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public LocationTriggerViewController (NSCoder coder)
			: base (coder)
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TriggerCreator = new LocationTriggerCreator (Trigger, Home);
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), LocationCell);
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), RegionStatusCell);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			viewIsDisplayed = true;
			var region = LocationTriggerCreator.TargetRegion;
			if (region != null) {
				var centerLocation = new CLLocation (region.Center.Latitude, region.Center.Longitude);
				geocoder.ReverseGeocodeLocation (centerLocation, (placemarks, error) => {
					// The geocoder took too long, we're not on this view any more.
					if (!viewIsDisplayed)
						return;

					if (error != null) {
						DisplayError (error);
						return;
					}

					if (placemarks != null) {
						var mostLikelyPlacemark = placemarks.FirstOrDefault ();
						if (mostLikelyPlacemark != null) {
							CNMutablePostalAddress address = CreatePostalAddress (mostLikelyPlacemark);
							var addressFormatter = new CNPostalAddressFormatter ();
							string addressString = addressFormatter.GetStringFromPostalAddress (address);
							localizedAddress = addressString.Replace ("\n", ", ");
							var section = NSIndexSet.FromIndex (2);
							TableView.ReloadSections (section, UITableViewRowAnimation.Automatic);
						}
					}
				});
			}
			TableView.ReloadData ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == selectLocationSegue) {
				var destinationVC = segue.IntendedDestinationViewController () as MapViewController;
				if (destinationVC == null)
					return;

				// Give the map the previous target region (if exists).
				destinationVC.TargetRegion = LocationTriggerCreator.TargetRegion;
				destinationVC.Delegate = LocationTriggerCreator;
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			viewIsDisplayed = false;
		}

		#region Table View Methods

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch (SectionForIndex ((int)section)) {
			case TriggerTableViewSection.Region:
				return 2;

			default:
				return base.RowsInSection (tableView, section);
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Region:
				return GetRegionStatusCell (tableView, indexPath);

			case TriggerTableViewSection.Location:
				return GetLocationCell (tableView, indexPath);

			default:
				return base.GetCell (tableView, indexPath);
			}
		}

		// Generates the single location cell.
		UITableViewCell GetLocationCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (LocationCell, indexPath);
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			if (LocationTriggerCreator.TargetRegion != null)
				cell.TextLabel.Text = localizedAddress ?? "Update Location";
			else
				cell.TextLabel.Text = "Set Location";

			return cell;
		}

		// Generates the cell which allow the user to select either 'on enter' or 'on exit'.
		UITableViewCell GetRegionStatusCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (RegionStatusCell, indexPath);
			cell.TextLabel.Text = RegionStatusTitles [indexPath.Row];
			cell.Accessory = (LocationTriggerCreator.TargetRegionStateIndex == indexPath.Row) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			switch (SectionForIndex (indexPath.Section)) {
			case TriggerTableViewSection.Location:
				PerformSegue (selectLocationSegue, this);
				break;

			case TriggerTableViewSection.Region:
				LocationTriggerCreator.TargetRegionStateIndex = indexPath.Row;
				var reloadIndexSet = NSIndexSet.FromIndex (indexPath.Section);
				tableView.ReloadSections (reloadIndexSet, UITableViewRowAnimation.Automatic);
				break;

			default:
				base.RowSelected (tableView, indexPath);
				break;
			}
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			switch (SectionForIndex ((int)section)) {
			case TriggerTableViewSection.Location:
				return "Location";

			case TriggerTableViewSection.Region:
				return "Region Status";

			default:
				return base.TitleForHeader (tableView, section);
			}
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			switch (SectionForIndex ((int)section)) {
			case TriggerTableViewSection.Region:
				return "This trigger can activate when you enter or leave a region. For example, when you arrive at home or when you leave work.";

			default:
				return base.TitleForFooter (tableView, section);
			}
		}

		#endregion

		protected override TriggerTableViewSection SectionForIndex (int index)
		{
			switch (index) {
			case 0:
				return TriggerTableViewSection.Name;

			case 1:
				return TriggerTableViewSection.Enabled;

			case 2:
				return TriggerTableViewSection.Location;

			case 3:
				return TriggerTableViewSection.Region;

			case 4:
				return TriggerTableViewSection.Conditions;

			case 5:
				return TriggerTableViewSection.ActionSets;

			default:
				return TriggerTableViewSection.None;
			}
		}

		// Constructs a `CNMutablePostalAddress` from a `CLPlacemark`
		static CNMutablePostalAddress CreatePostalAddress (CLPlacemark placemark)
		{
			return new CNMutablePostalAddress {
				Street = string.Format ("{0} {1}", placemark.SubThoroughfare ?? string.Empty, placemark.Thoroughfare ?? string.Empty),
				City = placemark.Locality ?? string.Empty,
				State = placemark.AdministrativeArea ?? string.Empty,
				PostalCode = placemark.PostalCode ?? string.Empty,
				Country = placemark.Country ?? string.Empty
			};
		}
	}
}