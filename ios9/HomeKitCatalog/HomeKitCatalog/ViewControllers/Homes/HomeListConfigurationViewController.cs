using System;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// Represents the sections in the `HomeListConfigurationViewController`.
	enum HomeListSection
	{
		Homes,
		PrimaryHome
	}

	/// <summary>
	/// A `HomeListViewController` subclass which allows the user to add and remove homes and set the primary home.
	/// </summary>
	partial class HomeListConfigurationViewController : HomeListViewController
	{
		static readonly NSString AddHomeCell = (NSString)"AddHomeCell";
		static readonly NSString NoHomesCell = (NSString)"NoHomesCell";
		static readonly NSString PrimaryHomeCell = (NSString)"PrimaryHomeCell";

		[Export ("initWithCoder:")]
		public HomeListConfigurationViewController (NSCoder coder)
			: base (coder)
		{
		}

		public HomeListConfigurationViewController (IntPtr handle)
			: base (handle)
		{
		}

		public HomeListConfigurationViewController ()
		{
		}

		#region Table View Methods

		public override nint NumberOfSections (UITableView tableView)
		{
			return Enum.GetNames (typeof(HomeListSection)).Length;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			switch ((HomeListSection)(int)section) {
			case HomeListSection.Homes:
				return Homes.Count + 1;
			case HomeListSection.PrimaryHome:
				return Math.Max (Homes.Count, 1);
			default:
				throw new InvalidOperationException ("Unexpected `HomeListSection` value");
			}
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (IndexPathIsAdd (indexPath))
				return tableView.DequeueReusableCell (AddHomeCell, indexPath);
			else if (Homes.Count == 0)
				return tableView.DequeueReusableCell (NoHomesCell, indexPath);

			var section = (HomeListSection)indexPath.Section;
			NSString reuseId;

			switch (section) {
			case HomeListSection.Homes:
				reuseId = HomeCell;
				break;
			case HomeListSection.PrimaryHome:
				reuseId = PrimaryHomeCell;
				break;
			default:
				throw new InvalidOperationException ("Unexpected `HomeListSection` value.");
			}

			var cell = tableView.DequeueReusableCell (reuseId, indexPath);
			var home = Homes [indexPath.Row];

			cell.TextLabel.Text = home.Name;

			var detailTextLabel = cell.DetailTextLabel;
			if (detailTextLabel != null)
				detailTextLabel.Text = SharedTextForHome (home);

			if (section == HomeListSection.PrimaryHome) {
				cell.Accessory = (home == HomeManager.PrimaryHome)
					? UITableViewCellAccessory.Checkmark
					: UITableViewCellAccessory.None;
			}

			return cell;
		}

		// Homes in the list section can be deleted. The add row cannot be deleted.
		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return GetSection (indexPath) == HomeListSection.Homes && !IndexPathIsAdd (indexPath);
		}

		// Provides subtext about the use of designating a "primary home".
		public override string TitleForHeader (UITableView tableView, nint section)
		{
			var isPrimeHome = (HomeListSection)(int)section == HomeListSection.PrimaryHome;
			return isPrimeHome ? "Primary Home" : null;

		}

		// If selecting a regular home, a segue will be performed.
		// If this method is called, the user either selected the 'add' row,
		// a primary home cell, or the `No Homes` cell.
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true);
			if (IndexPathIsAdd (indexPath))
				AddNewHome ();
			else if (IndexPathIsNone (indexPath))
				return;
			else if (GetSection (indexPath) == HomeListSection.PrimaryHome)
				UpdatePrimaryHome (Homes [indexPath.Row]);
		}

		// Removes the home from HomeKit if the row is deleted.
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete)
				RemoveHomeAtIndexPath (indexPath);
		}

		#endregion

		#region Helper Methods

		static HomeListSection GetSection (NSIndexPath indexPath)
		{
			return (HomeListSection)indexPath.Section;
		}

		// Updates the primary home in HomeKit and reloads the view.
		// If the home is already selected, no action is taken.
		void UpdatePrimaryHome (HMHome newPrimaryHome)
		{
			if (newPrimaryHome != HomeManager.PrimaryHome) {
				HomeManager.UpdatePrimaryHome (newPrimaryHome, error => {
					if (error != null)
						DisplayError (error);
					else
						DidUpdatePrimaryHome ();
				});
			}
		}

		// Reloads the 'primary home' section.
		void DidUpdatePrimaryHome ()
		{
			TableView.ReloadSections (NSIndexSet.FromIndex ((int)HomeListSection.PrimaryHome), UITableViewRowAnimation.Automatic);
		}

		// Removed the home at the specified index path from HomeKit and updates the view.
		void RemoveHomeAtIndexPath (NSIndexPath indexPath)
		{
			var home = Homes [indexPath.Row];
			DidRemoveHome (home);
			HomeManager.RemoveHome (home, error => {
				if (error != null) {
					DisplayError (error);
					DidAddHome (home);
				}
			});
		}

		// Presents an alert controller so the user can provide a name. If committed, the home is created.
		void AddNewHome ()
		{
			this.PresentAddAlertWithAttributeType ("Home", "Apartment", null, AddHomeWithName);
		}

		// Removes a home from the List and updates the view.
		protected override void DidRemoveHome (HMHome home)
		{
			var index = Homes.IndexOf (home);
			if (index < 0)
				return;

			var indexPath = NSIndexPath.FromRowSection (index, (int)HomeListSection.Homes);
			Homes.RemoveAt (index);
			var primaryIndexPath = NSIndexPath.FromRowSection (index, (int)HomeListSection.PrimaryHome);

			// If there aren't any homes, we still want one cell to display 'No Homes'.
			// Just reload.

			TableView.BeginUpdates ();
			if (Homes.Count == 0)
				TableView.ReloadRows (new [] { primaryIndexPath }, UITableViewRowAnimation.Fade);
			else
				TableView.DeleteRows (new [] { primaryIndexPath }, UITableViewRowAnimation.Automatic);

			TableView.DeleteRows (new [] { indexPath }, UITableViewRowAnimation.Automatic);
			TableView.EndUpdates ();
		}

		protected override void DidAddHome (HMHome home)
		{
			Homes.Add (home);
			SortHomes ();

			var newHomeIndex = Homes.IndexOf (home);
			if (newHomeIndex < 0)
				return;

			var indexPath = NSIndexPath.FromRowSection (newHomeIndex, (int)HomeListSection.Homes);

			var primaryIndexPath = NSIndexPath.FromRowSection (newHomeIndex, (int)HomeListSection.PrimaryHome);

			TableView.BeginUpdates ();

			if (Homes.Count == 1)
				TableView.ReloadRows (new [] { primaryIndexPath }, UITableViewRowAnimation.Fade);
			else
				TableView.InsertRows (new [] { primaryIndexPath }, UITableViewRowAnimation.Automatic);

			TableView.InsertRows (new [] { indexPath }, UITableViewRowAnimation.Automatic);
			TableView.EndUpdates ();
		}

		void AddHomeWithName (string name)
		{
			HomeManager.AddHome (name, (newHome, error) => {
				if (error != null) {
					DisplayError (error);
					return;
				}

				DidAddHome (newHome);
			});
		}

		// returns:  `true` if the index path is the 'add row'; `false` otherwise.
		bool IndexPathIsAdd (NSIndexPath indexPath)
		{
			return GetSection (indexPath) == HomeListSection.Homes && indexPath.Row == Homes.Count;
		}

		// returns:  `true` if the index path is the 'No Homes' cell; `false` otherwise.
		bool IndexPathIsNone (NSIndexPath indexPath)
		{
			return GetSection (indexPath) == HomeListSection.PrimaryHome && Homes.Count == 0;
		}

		#endregion

		#region HMHomeDelegate Methods

		// Finds the home in the Homes property and reloads the corresponding row.
		protected override void HomeDidUpdateName (HMHome home)
		{
			var index = Homes.IndexOf (home);
			if (index >= 0) {
				var listIndexPath = NSIndexPath.FromRowSection (index, (int)HomeListSection.Homes);
				var primaryIndexPath = NSIndexPath.FromRowSection (index, (int)HomeListSection.PrimaryHome);

				TableView.ReloadRows (new [] { listIndexPath, primaryIndexPath }, UITableViewRowAnimation.Automatic);
			} else {
				// Just reload the data since we don't know the index path.
				TableView.ReloadData ();
			}
		}

		#endregion

		#region HMHomeManagerDelegate Methods

		[Export ("homeManagerDidUpdatePrimaryHome:")]
		public void DidUpdatePrimaryHome (HMHomeManager manager)
		{
			DidUpdatePrimaryHome ();
		}

		#endregion
	}
}