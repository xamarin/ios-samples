using System;
using System.Collections.Generic;

using CoreFoundation;
using Foundation;
using HomeKit;
using UIKit;

namespace HomeKitCatalog
{
	// A view controller that lists rooms within a home and allows the user to add the rooms to a provided zone.
	public partial class AddRoomViewController : HMCatalogViewController
	{
		static readonly NSString RoomCell = (NSString)"RoomCell";

		readonly List<HMRoom> displayedRooms = new List<HMRoom> ();
		readonly List<HMRoom> selectedRooms = new List<HMRoom> ();

		public HMZone HomeZone { get; set; }

		public AddRoomViewController (IntPtr handle)
			: base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public AddRoomViewController (NSCoder coder)
			: base (coder)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			Title = HomeZone.Name;
			ResetDisplayedRooms ();
		}

		// Adds the selected rooms to the zone and dismisses the view.
		[Export ("dismiss:")]
		void dismiss (NSObject sender)
		{
			AddSelectedRoomsToZoneWithCompletionHandler (() => DismissViewController (true, null));
		}

		// Creates a dispatch group, adds all of the rooms to the zone, and runs the provided completion once all rooms have been added.
		void AddSelectedRoomsToZoneWithCompletionHandler (Action completionHandler)
		{
			var group = DispatchGroup.Create ();
			foreach (var room in selectedRooms) {
				group.Enter ();
				HomeZone.AddRoom (room, error => {
					if (error != null)
						DisplayError (error);
					group.Leave ();
				});
			}
			group.Notify (DispatchQueue.MainQueue, completionHandler);
		}

		#region Table View Methods

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return displayedRooms.Count;
		}

		// returns:  A cell that includes the name of a room and a checkmark if it's intended to be added to the zone.
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (RoomCell, indexPath);
			var room = displayedRooms [indexPath.Row];

			cell.TextLabel.Text = room.Name;
			cell.Accessory = selectedRooms.Contains (room) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

			return cell;
		}

		// Adds the selected room to the selected rooms array and reloads that cell
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var room = displayedRooms [indexPath.Row];

			var index = selectedRooms.IndexOf (room);
			if (index >= 0)
				selectedRooms.RemoveAtIndex (index);
			else
				selectedRooms.Add (room);

			tableView.ReloadRows (new []{ indexPath }, UITableViewRowAnimation.Automatic);
		}

		// Resets the list of displayed rooms and reloads the table.
		void ResetDisplayedRooms ()
		{
			displayedRooms.Clear ();
			displayedRooms.AddRange (Home.RoomsNotAlreadyInZone (HomeZone, selectedRooms));
			if (displayedRooms.Count == 0)
				DismissViewController (true, null);
			else
				TableView.ReloadData ();
		}

		#endregion

		#region HMHomeDelegate Methods

		// If our zone was removed, dismiss this view.
		[Export ("home:didRemoveZone:")]
		public void DidRemoveZone (HMHome home, HMZone zone)
		{
			if (zone == HomeZone)
				DismissViewController (true, null);
		}

		// If our zone was renamed, reset our title.
		[Export ("home:didUpdateNameForZone:")]
		public void DidUpdateNameForZone (HMHome home, HMZone zone)
		{
			if (zone == HomeZone)
				Title = zone.Name;
		}

		// All home updates reset the displayed homes and reload the view.

		[Export ("home:didUpdateNameForRoom:")]
		public void DidUpdateNameForRoom (HMHome home, HMRoom room)
		{
			ResetDisplayedRooms ();
		}

		[Export ("home:didAddRoom:")]
		public void DidAddRoom (HMHome home, HMRoom room)
		{
			ResetDisplayedRooms ();
		}

		[Export ("home:didRemoveRoom:")]
		public void DidRemoveRoom (HMHome home, HMRoom room)
		{
			ResetDisplayedRooms ();
		}

		[Export ("home:didAddRoom:toZone:")]
		public void DidAddRoomToZone (HMHome home, HMRoom room, HMZone zone)
		{
			ResetDisplayedRooms ();
		}

		[Export ("home:didRemoveRoom:fromZone:")]
		public void DidRemoveRoomFromZone (HMHome home, HMRoom room, HMZone zone)
		{
			ResetDisplayedRooms ();
		}

		#endregion
	}
}