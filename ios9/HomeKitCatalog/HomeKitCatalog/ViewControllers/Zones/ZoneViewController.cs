using System;
using System.Collections.Generic;

using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A view controller that lists the rooms within a provided zone.
	public partial class ZoneViewController : HMCatalogViewController
	{
		static readonly NSString RoomCell = (NSString)"RoomCell";
		static readonly NSString AddCell = (NSString)"AddCell";
		static readonly NSString DisabledAddCell = (NSString)"DisabledAddCell";
		const string AddRoomsSegue = "Add Rooms";

		readonly List<HMRoom> rooms = new List<HMRoom> ();

		public HMZone HomeZone { get; set; }

		public ZoneViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ZoneViewController (NSCoder coder)
			: base (coder)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			Title = HomeZone.Name;
			ReloadData ();
		}

		// If our data is invalid, pop the view controller.
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			if (ShouldPopViewController ())
				NavigationController.PopViewController (true);
		}

		// Provide the zone to `AddRoomViewController`.
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == AddRoomsSegue) {
				var addViewController = (AddRoomViewController)segue.IntendedDestinationViewController ();
				addViewController.HomeZone = HomeZone;
			}
		}

		#region Helper Methods

		// Resets the internal list of rooms and reloads the table view.
		void ReloadData ()
		{
			rooms.Clear ();
			rooms.AddRange (HomeZone.Rooms);
			rooms.SortByLocalizedName (r => r.Name);
			TableView.ReloadData ();
		}


		// Sorts the internal list of rooms by localized name.
		void SortRooms ()
		{
			rooms.SortByLocalizedName (r => r.Name);
		}

		// returns:  The `NSIndexPath` where the 'Add Cell' should be located.
		NSIndexPath AddIndexPath ()
		{
			return NSIndexPath.FromRowSection (rooms.Count, 0);
		}

		bool IndexPathIsAdd (NSIndexPath indexPath)
		{
			return indexPath.Row == AddIndexPath ().Row;
		}

		// Adds a room to the internal array of rooms and inserts new row into the table view.
		void DidAddRoom (HMRoom room)
		{
			rooms.Add (room);
			SortRooms ();

			var newRoomIndex = rooms.IndexOf (room);
			if (newRoomIndex >= 0) {
				var newRoomIndexPath = NSIndexPath.FromRowSection (newRoomIndex, 0);
				TableView.InsertRows (new []{ newRoomIndexPath }, UITableViewRowAnimation.Automatic);
			}

			ReloadAddIndexPath ();
		}

		// Removes a room from the internal array of rooms and deletes the row from the table view.
		void DidRemoveRoom (HMRoom room)
		{
			var roomIndex = rooms.IndexOf (room);
			if (roomIndex >= 0) {
				rooms.RemoveAtIndex (roomIndex);
				var roomIndexPath = NSIndexPath.FromRowSection (roomIndex, 0);
				TableView.DeleteRows (new []{ roomIndexPath }, UITableViewRowAnimation.Automatic);
			}

			ReloadAddIndexPath ();
		}

		void ReloadAddIndexPath ()
		{
			TableView.ReloadRows (new []{ AddIndexPath () }, UITableViewRowAnimation.Automatic);
		}

		// Reloads the cell corresponding a given room.
		void DidUpdateRoom (HMRoom room)
		{
			var roomIndex = rooms.IndexOf (room);
			if (roomIndex >= 0) {
				var roomIndexPath = NSIndexPath.FromRowSection (roomIndex, 0);
				TableView.ReloadRows (new []{ roomIndexPath }, UITableViewRowAnimation.Automatic);
			}
		}

		// Removes a room from HomeKit and updates the view.
		void RemoveRoom (HMRoom room)
		{
			DidRemoveRoom (room);
			HomeZone.RemoveRoom (room, error => {
				if (error != null) {
					DisplayError (error);
					DidAddRoom (room);
				}
			});
		}

		// returns: `true` if our current home no longer exists, `false` otherwise.
		bool ShouldPopViewController ()
		{
			foreach (var zone in Home.Zones) {
				if (zone == HomeZone)
					return false;
			}
			return true;
		}

		// returns:  `true` if more rooms can be added to this zone; `false` otherwise.
		bool CanAddRoom ()
		{
			return rooms.Count < Home.Rooms.Length;
		}

		#endregion

		#region Table View Methods

		// returns:  The number of rooms in the zone, plus 1 for the 'add' row.
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return rooms.Count + 1;
		}

		// returns:  A cell containing the name of an HMRoom.
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (IndexPathIsAdd (indexPath)) {
				var reuseIdentifier = Home.IsAdmin () && CanAddRoom () ? AddCell : DisabledAddCell;
				return tableView.DequeueReusableCell (reuseIdentifier, indexPath);
			}

			var cell = tableView.DequeueReusableCell (RoomCell, indexPath);
			cell.TextLabel.Text = rooms [indexPath.Row].Name;
			return cell;
		}

		// returns: `true` if the cell is anything but an 'add' cell; `false` otherwise.
		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return Home.IsAdmin () && !IndexPathIsAdd (indexPath);
		}

		// Deletes the room at the provided index path.
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete) {
				var room = rooms [indexPath.Row];
				RemoveRoom (room);
			}
		}

		#endregion

		#region HMHomeDelegate Methods

		// If our zone was removed, pop the view controller.
		[Export ("home:didRemoveZone:")]
		public void DidRemoveZone (HMHome home, HMZone zone)
		{
			if (zone == HomeZone)
				NavigationController.PopViewController (true);
		}

		// If our zone was renamed, update the title.
		[Export ("home:didUpdateNameForZone:")]
		public void DidUpdateNameForZone (HMHome home, HMZone zone)
		{
			if (zone == HomeZone)
				Title = zone.Name;
		}

		// Update the row for the room.
		[Export ("home:didUpdateNameForRoom:")]
		public void DidUpdateNameForRoom (HMHome home, HMRoom room)
		{
			DidUpdateRoom (room);
		}

		// A room has been added, we may be able to add it to the zone. Reload the 'addIndexPath'
		[Export ("home:didAddRoom:")]
		public void DidAddRoom (HMHome home, HMRoom room)
		{
			ReloadAddIndexPath ();
		}

		// A room has been removed, attempt to remove it from the room.
		// This will always reload the 'addIndexPath'.
		[Export ("home:didRemoveRoom:")]
		public void DidRemoveRoom (HMHome home, HMRoom room)
		{
			DidRemoveRoom (room);
		}

		// If the room was added to our zone, add it to the view.
		[Export ("home:didAddRoom:toZone:")]
		public void DidAddRoomToZone (HMHome home, HMRoom room, HMZone zone)
		{
			if (zone == HomeZone)
				DidAddRoom (room);
		}

		// If the room was removed from our zone, remove it from the view.
		[Export ("home:didRemoveRoom:fromZone:")]
		public void DidRemoveRoomFromZone (HMHome home, HMRoom room, HMZone zone)
		{
			if (zone == HomeZone)
				DidRemoveRoom (room);
		}

		#endregion
	}
}
